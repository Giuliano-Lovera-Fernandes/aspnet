using EstoqueWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace EstoqueWeb.Controllers
{
    public class EnderecoController: Controller
    {
        //Criar um atributo somente leitura para guardar
        private readonly EstoqueWebContext _context; 
        public EnderecoController(EstoqueWebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? cid)
        {
            /* Caso o id do cliente possua um valor o procuro no banco de dados */
            if (cid.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(cid);
                /* Caso seja possível o carregar, vou na coleção de Enderecos e a carrego */
                if (cliente != null)
                {
                    /* O load serve para carregar uma coleção específica de endereços, a fim de evitar perda de performance. */
                   _context.Entry(cliente).Collection(c => c.Enderecos).Load();
                   /* Retornamos os enderecos do cliente. */
                   ViewBag.Cliente = cliente; 
                   return View(cliente.Enderecos);                                
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não foi encontrado. ", TipoMensagem.Erro);
                    return RedirectToAction("Index", "Cliente");
                }
                                   
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Só é possível mostrar endereços de um cliente específico. ", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(int? cid, int? eid)
        {
            if (cid.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(cid);
                if (cliente != null)
                {
                    ViewBag.Cliente = cliente;
                    if (eid.HasValue)
                    {
                        _context.Entry(cliente).Collection(c => c.Enderecos).Load();
                        var endereco = cliente.Enderecos.FirstOrDefault(e => e.IdEndereco == eid);
                        if (endereco != null)
                        {
                            /* View Cadastrar será exibida com os dados do endereço encontrado para o cliente */
                            return View(endereco);
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Endereço não encontrado.", TipoMensagem.Erro);
                        }
                    }
                    else
                    {
                        return View(new EnderecoModel());                            
                    }                    
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado", TipoMensagem.Erro);
                    return RedirectToAction("Index");
                }                
            }
            else
            {
                /* Caso o id não seja passado */
                TempData["mensagem"] = MensagemModel.Serializar("Nenhum proprietário de endereços foi informado", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente"); 
            }
            return View(new EnderecoModel());
        }

        private bool EnderecoExiste(int cid, int eid)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdUsuario == cid);

            if (cliente != null)
            {
                return cliente.Enderecos.Any(e => e.IdEndereco == eid);  
            }
            else
            {
                return false;    
            }            
        }

        [HttpPost]
        /* Indica que o endereco será alimentado pelo formulário */
        public async Task<IActionResult> Cadastrar([FromForm] int? idUsuario, [FromForm] EnderecoModel endereco)
        {
            if (idUsuario.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(idUsuario);
                ViewBag.Cliente = cliente;
                if (ModelState.IsValid)
                {
                    if (cliente.Enderecos.Count() == 0)
                    {
                        endereco.Selecionado = true;
                    }
                    endereco.Cep = ObterCepNormalizado(endereco.Cep); 
                    if (endereco.IdEndereco > 0)
                    {
                        if (endereco.Selecionado)
                        {
                            cliente.Enderecos.ToList().ForEach(e => e.Selecionado = false);
                        }

                        if (EnderecoExiste(idUsuario.Value, endereco.IdEndereco))
                        {
                            var enderecoAtual = cliente.Enderecos
                                .FirstOrDefault(e => e.IdEndereco == endereco.IdEndereco);
                            _context.Entry(enderecoAtual).CurrentValues.SetValues(endereco); 
                            if (_context.Entry(enderecoAtual).State == EntityState.Unchanged)
                            {
                                TempData["mensagem"] = MensagemModel.Serializar("Nenhum dado de endereço foi alterado.");
                            } 
                            else
                            {
                                if (await _context.SaveChangesAsync() > 0)
                                {
                                    TempData["mensagem"] = MensagemModel.Serializar("Endereço alterado com sucesso.");
                                }
                                else
                                {
                                    TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar endereço.");
                                }
                            }  
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Endereço não encontrado.", TipoMensagem.Erro);
                        }

                    }
                    else
                    {
                        var idEndereco = cliente.Enderecos.Count() > 0 ?
                            cliente.Enderecos.Max(e => e.IdEndereco) + 1 :
                            1;
                        endereco.IdEndereco = idEndereco;
                        if (endereco != null)
                        {
                            _context.Clientes.FirstOrDefault(c => c.IdUsuario == idUsuario).Enderecos.Add(endereco);
                            if (await _context.SaveChangesAsync() > 0)
                            {
                                TempData["mensagem"] = MensagemModel.Serializar("Endereço cadastrado com sucesso.");
                            }
                        }
                            
                    }
                    return RedirectToAction("Index", "Endereco", new {cid = idUsuario});
                }
                else
                { 
                    return View(endereco);
                }
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Nenhum proprietário de endereço foi informado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "cliente");
            }
        }

        private string ObterCepNormalizado(string cep)
        {
            string cepNormalizado = cep.Replace("-", "").Replace(".", "").Trim();
            return cepNormalizado.Insert(5, "-");
        }                                 

        [HttpGet]
        public async Task<IActionResult> Excluir ( int? cid, int? eid)
        {
            if(!cid.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("cliente não informado", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }

            if(!eid.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Endereço não informado", TipoMensagem.Erro);
                return RedirectToAction("Index", new {cid = cid});
            }

            var cliente = await _context.Clientes.FindAsync(cid);

            var endereco = cliente.Enderecos.FirstOrDefault(e => e.IdEndereco == eid);
            if (endereco == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Endereco não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", new {cid = cid});
            }

            ViewBag.Cliente = cliente;
            return View(endereco);
        }
        [HttpPost]
        public async Task<IActionResult> Excluir (int idUsuario, int idEndereco)
        {
            var cliente = await _context.Clientes.FindAsync(idUsuario);
            var endereco = cliente.Enderecos.FirstOrDefault(e => e.IdEndereco == idEndereco); 
            if (endereco != null)
            {
                cliente.Enderecos.Remove(endereco);
                if (await _context.SaveChangesAsync() > 0)
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Endereço excluído com sucesso.");
                    if (endereco.Selecionado && cliente.Enderecos.Count() > 0)
                    {
                        cliente.Enderecos.FirstOrDefault().Selecionado = true; 
                        await _context.SaveChangesAsync();       
                    }
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o endereço.", TipoMensagem.Erro);                                   
                }
                return RedirectToAction("Index", new {cid = idUsuario});
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Endereço não encontrado.", TipoMensagem.Erro); 
                return RedirectToAction("Index", new {cid = idUsuario});                                  
            }
        }
    }    
}