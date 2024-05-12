using EstoqueWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace EstoqueWeb.Controllers
{
    public class PedidoController: Controller
    {
        //Criar um atributo somente leitura para guardar
        private readonly EstoqueWebContext _context; 
        public PedidoController(EstoqueWebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? cid)
        {
            if (cid.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(cid);
                if (cliente != null)
                {
                    /* Pega todos os pedidos de um cliente e ordena-os em ordem descendente pelo id do pedido (pedido mais recente mostrado primeiro)*/
                    var pedidos = await _context.Pedidos
                        .Where(p => p.IdCliente == cid)
                        .OrderByDescending(x => x.IdPedido)
                        .AsNoTracking().ToListAsync();

                    ViewBag.Cliente = cliente;
                    return View(pedidos);    
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado", TipoMensagem.Erro);
                        return RedirectToAction("Index", "Cliente");    
                }
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado", TipoMensagem.Erro);                
                    return RedirectToAction("Index", "Cliente");
            }            
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar( int? cid)
        {
            if (cid.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(cid);
                var enderecoEntrega = new EnderecoModel();
                enderecoEntrega = cliente.Enderecos.FirstOrDefault(e => e.Selecionado);

                if (cliente != null)
                {
                    _context.Entry(cliente).Collection(c => c.Pedidos).Load();
                    PedidoModel pedido = null;
                    
                    //Se tem algum pedido para esse cliente e cuja data do pedido não tem um valor, significa que o pedido está em aberto.
                    if (_context.Pedidos.Any(p => p.IdCliente == cid && !p.DataPedido.HasValue))
                    { 
                        //pegando o pedido do cliente, cuja data pedido está em aberto.                       
                        pedido = await _context.Pedidos
                            .FirstOrDefaultAsync(p => p.IdCliente == cid && !p.DataPedido.HasValue);
                    }
                    else
                    {
                        /* var enderecoAtual = cliente.Enderecos
                                .FirstOrDefault(e => e.IdEndereco == pedido.EnderecoEntrega.IdEndereco);
                        pedido.EnderecoEntrega = enderecoAtual; */
                        //enderecoEntrega = cliente.Enderecos.Where(e => e.Selecionado).ToList();
                        //ICollection<EnderecoModel> enderecosSelecionados = cliente.Enderecos?.FirstOrDefault(e => e.Selecionado);

                        //pedido.EnderecoEntrega.Logradouro = enderecosSelecionados.

                        //pedido = new PedidoModel { IdCliente = cid.Value, ValorTotal = 0, EnderecoEntrega = enderecosSelecionados };
                        pedido = new PedidoModel { IdCliente = cid.Value , ValorTotal = 0, EnderecoEntrega = enderecoEntrega};

                        cliente.Pedidos.Add(pedido);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("Index", "ItemPedido", new { ped = pedido.IdPedido });
                }
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado", TipoMensagem.Erro);
            return RedirectToAction("Index", "Cliente");
        }

        private bool PedidoExiste(int id)
        {
            return _context.Pedidos.Any(c => c.IdPedido == id);    
        }

        [HttpPost]
        /* Indica que o pedido será alimentado pelo formulário */
        public async Task<IActionResult> Cadastrar([FromQuery(Name = "item")] int? id, [FromForm] PedidoModel pedido)
        {
            //
            if (pedido.IdPedido != 0)
                id = pedido.IdPedido;
            /* São aplicadas todas as restrições definidas, entenda-se, Se os critérios estabelecidos forem satisfeitos */
            if (ModelState.IsValid)
            {
                //Se o id existe é sinal que é uma alteração
                if (id.HasValue)
                {
                    //Busca pelo id e retorna verdadeiro se o encontra
                    if (PedidoExiste(id.Value))
                    {
                        //atualiza a pedido, salva as alterações
                        _context.Update(pedido);

                        //define se ocorreu alguma mudança
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Pedido alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar pedido.", TipoMensagem.Erro);
                        }
                    }                    
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrada.", TipoMensagem.Erro);
                    }
                }
                else
                {
                    //Caso o id não exista, ocorre uma inclusão
                    _context.Add(pedido);
                    // _context.Pedidos.Add(pedido);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Pedido cadastrada com sucesso.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar pedido.", TipoMensagem.Erro);
                    }
                    //return RedirectToAction(nameof(Index));
                    
                }
                //return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            //Faz com que permaneça na view do formulário
            return View(pedido);                
        }

        [HttpGet]
        public async Task<IActionResult> Excluir ( int? id)
        {
            if(!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não informada", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrada.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }
        [HttpPost]
        public async Task<IActionResult> Excluir (int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id); 
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
                if (await _context.SaveChangesAsync() > 0)
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Pedido excluída com sucesso.");
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir a pedido.", TipoMensagem.Erro);                                   
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrada.", TipoMensagem.Erro); 
                return RedirectToAction(nameof(Index));                                  
            }
        }
    }    
}