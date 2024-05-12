using EstoqueWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace EstoqueWeb.Controllers
{
    public class ClienteController: Controller
    {
        //Criar um atributo somente leitura para guardar
        private readonly EstoqueWebContext _context; 
        public ClienteController(EstoqueWebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes.AsNoTracking().ToListAsync();
            return View(clientes.OrderBy(c => c.Nome));
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar([FromQuery(Name = "item")] int? id)
        {
            /* id como parâmetro opcional
            Se o id tem um valor, busca-se o cliente*/ 
            if (id.HasValue)
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    /* Cliente não foi encontrado é retornado um erro como mensagem e o usuário é redirecionado para ação Index */
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado.", TipoMensagem.Erro);
                    return RedirectToAction("Index");
                }
                /* Caso o cliente, seja encontrado, ele é retornado */
                return View(cliente);
            }
            return View(new ClienteModel());
        }

        private bool ClienteExiste(int id)
        {           
            return _context.Clientes.Any(c => c.IdUsuario == id);    
        }

        [HttpPost]
        /* Indica que o cliente será alimentado pelo formulário */
        public async Task<IActionResult> Cadastrar([FromQuery(Name = "item")] int? id, [FromForm] ClienteModel cliente)
        {
            //
            if (cliente.IdUsuario != 0)
                id = cliente.IdUsuario;
            /* São aplicadas todas as restrições, regras de validação, que foram definidas nos modelos pelos DataAnnotations, entenda-se, 
            Se os critérios estabelecidos forem satisfeitos e se relaciona ao modelo dessa ação. */
            if (ModelState.IsValid)
            {
                //Se o id existe é sinal que é uma alteração, caso contrário é uma inclusão
                if (id.HasValue)
                {
                    //Busca pelo id e retorna verdadeiro se o encontra
                    if (ClienteExiste(id.Value))
                    {
                        //atualiza a cliente, salva as alterações na memória
                        _context.Update(cliente);

                        /* Evita que a senha seja atualizada em branco */
                         _context.Entry(cliente).Property(c => c.Senha).IsModified = true;

                        //define se ocorreu alguma mudança, se mais de um registro foi afetado pelas alterações.
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Cliente alterado com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar cliente.", TipoMensagem.Erro);
                        }
                    }
                    /* Caso nenhum cliente foi encontrado com o id passado */                    
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado.", TipoMensagem.Erro);
                    }
                }
                else
                {
                    //Caso o id não exista, ocorre uma inclusão
                    _context.Add(cliente);
                    // _context.Clientes.Add(cliente), adiciona um novo cliente;
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Cliente cadastrado com sucesso.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar cliente.", TipoMensagem.Erro);
                    }
                    //return RedirectToAction(nameof(Index));
                    
                }
                //return RedirectToAction("Index"); Redirecionamento do usuário para a listagem de clientes.
                return RedirectToAction(nameof(Index));
            }
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Erro: {modelError.ErrorMessage}");
            }
            //Checagem do model state. Faz com que permaneça na view do formulário, contendo todas os erros
            return View(cliente);                
        }

        [HttpGet]
        public async Task<IActionResult> Excluir ( int? id)
        {
            if(!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não informado", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }
        [HttpPost]
        public async Task<IActionResult> Excluir (int id)
        {
            var cliente = await _context.Clientes.FindAsync(id); 
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                if (await _context.SaveChangesAsync() > 0)
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Cliente excluído com sucesso.");
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o cliente.", TipoMensagem.Erro);                                   
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Cliente não encontrado.", TipoMensagem.Erro); 
                return RedirectToAction(nameof(Index));                                  
            }
        }
    }    
}