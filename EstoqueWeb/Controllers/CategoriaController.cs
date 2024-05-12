using EstoqueWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace EstoqueWeb.Controllers
{
    public class CategoriaController: Controller
    {
        //Criar um atributo somente leitura para guardar o contexto do banco de dados, assim qualquer ação pode fazer uso do banco de dados.
        private readonly EstoqueWebContext _context; 
        public CategoriaController(EstoqueWebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias.AsNoTracking().ToListAsync();
            return View(categorias.OrderBy(c => c.Nome));
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar([FromQuery(Name = "item")] int? id)
        {
            if (id.HasValue)
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                {
                    return NotFound();
                }
                return View(categoria);
            }
            return View(new CategoriaModel());
        }

        private bool CategoriaExiste(int id)
        {
            return _context.Categorias.Any(c => c.IdCategoria == id);    
        }

        [HttpPost]
        /* Indica que o categoria será alimentado pelo formulário */
        public async Task<IActionResult> Cadastrar([FromQuery(Name = "item")] int? id, [FromForm] CategoriaModel categoria)
        {
            //
            if (categoria.IdCategoria != 0)
                id = categoria.IdCategoria;
            /* São aplicadas todas as restrições definidas, entenda-se, Se os critérios estabelecidos forem satisfeitos */
            if (ModelState.IsValid)
            {
                //Se o id existe é sinal que é uma alteração
                if (id.HasValue)
                {
                    //Busca pelo id e retorna verdadeiro se o encontra
                    if (CategoriaExiste(id.Value))
                    {
                        //atualiza a categoria, salva as alterações
                        _context.Update(categoria);

                        //define se ocorreu alguma mudança
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Categoria alterada com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar categoria.", TipoMensagem.Erro);
                        }
                    }                    
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Categoria não encontrada.", TipoMensagem.Erro);
                    }
                }
                else
                {
                    //Caso o id não exista, ocorre uma inclusão
                    _context.Add(categoria);
                    // _context.Categorias.Add(categoria);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Categoria cadastrada com sucesso.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar categoria.", TipoMensagem.Erro);
                    }
                    //return RedirectToAction(nameof(Index));
                    
                }
                //return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            //Faz com que permaneça na view do formulário
            return View(categoria);                
        }

        [HttpGet]
        public async Task<IActionResult> Excluir ( int? id)
        {
            if(!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Categoria não informada", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Categoria não encontrada.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }
        [HttpPost]
        public async Task<IActionResult> Excluir (int id)
        {
            var categoria = await _context.Categorias.FindAsync(id); 
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                if (await _context.SaveChangesAsync() > 0)
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Categoria excluída com sucesso.");
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir a categoria.", TipoMensagem.Erro);                                   
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Categoria não encontrada.", TipoMensagem.Erro); 
                return RedirectToAction(nameof(Index));                                  
            }
        }
    }    
}