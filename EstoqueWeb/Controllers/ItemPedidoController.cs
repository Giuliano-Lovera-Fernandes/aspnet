using EstoqueWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace EstoqueWeb.Controllers
{
    public class ItemPedidoController: Controller
    {
        //Criar um atributo somente leitura para guardar
        private readonly EstoqueWebContext _context; 
        public ItemPedidoController(EstoqueWebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? ped)
        {
            if (ped.HasValue)
            {
                /* Para mostrar uma listagem de pedidos eu preciso saber quem é o dono dos itens que vem no parâmetro ped
                
                verificação do pedido com esse id */
                if (_context.Pedidos.Any(p => p.IdPedido == ped))
                {
                    /* Tenho Pedido que por sua vez tem ItemPedido, que por sua vez, tem um produto, o ThenInclude atua sobre o que foi carregado no Include anterior */
                    /* São carregados os pedidos, incluindo vários objetos associados a ele, o que inclui o cliente,  os itens e os produtos*/
                    var pedido = await _context.Pedidos
                        .Include(p => p.Cliente)
                        .Include(p => p.ItensPedido.OrderBy(i => i.Produto.Nome))
                        .ThenInclude(i => i.Produto)
                        .FirstOrDefaultAsync(p => p.IdPedido == ped);

                    ViewBag.Pedido = pedido;
                    return View(pedido.ItensPedido);    
                }
                /* Repare que por possuir um return, nem se precisa do else */
                TempData["mensagem"] = MensagemModel.Serializar("Pedido não encontrado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
            }
            TempData["mensagem"] = MensagemModel.Serializar("Pedido não informado.", TipoMensagem.Erro);
                return RedirectToAction("Index", "Cliente");
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar([FromQuery(Name = "item")] int? id)
        {
            if (id.HasValue)
            {
                var itemPedido = await _context.ItensPedidos.FindAsync(id);
                if (itemPedido == null)
                {
                    return NotFound();
                }
                return View(itemPedido);
            }
            return View(new ItemPedidoModel());
        }

        private bool ItemPedidoExiste(int id)
        {
            return _context.ItensPedidos.Any(c => c.IdPedido == id);    
        }

        [HttpPost]
        /* Indica que o itemPedido será alimentado pelo formulário */
        public async Task<IActionResult> Cadastrar([FromQuery(Name = "item")] int? id, [FromForm] ItemPedidoModel itemPedido)
        {
            //
            if (itemPedido.IdPedido != 0)
                id = itemPedido.IdPedido;
            /* São aplicadas todas as restrições definidas, entenda-se, Se os critérios estabelecidos forem satisfeitos */
            if (ModelState.IsValid)
            {
                //Se o id existe é sinal que é uma alteração
                if (id.HasValue)
                {
                    //Busca pelo id e retorna verdadeiro se o encontra
                    if (ItemPedidoExiste(id.Value))
                    {
                        //atualiza a itemPedido, salva as alterações
                        _context.Update(itemPedido);

                        //define se ocorreu alguma mudança
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("ItemPedido alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar itemPedido.", TipoMensagem.Erro);
                        }
                    }                    
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("ItemPedido não encontrada.", TipoMensagem.Erro);
                    }
                }
                else
                {
                    //Caso o id não exista, ocorre uma inclusão
                    _context.Add(itemPedido);
                    // _context.ItemPedidos.Add(itemPedido);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("ItemPedido cadastrada com sucesso.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar itemPedido.", TipoMensagem.Erro);
                    }
                    //return RedirectToAction(nameof(Index));
                    
                }
                //return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            //Faz com que permaneça na view do formulário
            return View(itemPedido);                
        }

        [HttpGet]
        public async Task<IActionResult> Excluir ( int? id)
        {
            if(!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("ItemPedido não informada", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var itemPedido = await _context.ItensPedidos.FindAsync(id);
            if (itemPedido == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("ItemPedido não encontrada.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(itemPedido);
        }
        [HttpPost]
        public async Task<IActionResult> Excluir (int id)
        {
            var itemPedido = await _context.ItensPedidos.FindAsync(id); 
            if (itemPedido != null)
            {
                _context.ItensPedidos.Remove(itemPedido);
                if (await _context.SaveChangesAsync() > 0)
                {
                    TempData["mensagem"] = MensagemModel.Serializar("ItemPedido excluída com sucesso.");
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir a itemPedido.", TipoMensagem.Erro);                                   
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("ItemPedido não encontrada.", TipoMensagem.Erro); 
                return RedirectToAction(nameof(Index));                                  
            }
        }
    }    
}