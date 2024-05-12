using EstoqueWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace EstoqueWeb.Controllers
{
    public class ProdutoController: Controller
    {
        //Criar um atributo somente leitura para guardar
        private readonly EstoqueWebContext _context; 
        public ProdutoController(EstoqueWebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {            
            //var produtos = await _context.Produtos.AsNoTracking().ToListAsync();
            //return View(produtos.OrderBy(c => c.Nome));
            //A consulta inclui os objetos categorias associados aos produtos.
            return View(await _context.Produtos.OrderBy(p => p.Nome).Include(p => p.Categoria).AsNoTracking().ToListAsync());

        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar( int? id)
        {
            var categorias = _context.Categorias.OrderBy(c => c.Nome).AsNoTracking().ToList();

            //Preencher opçõe de um elemento select(fonte de dados, propriedade usada para valor de cada opção, propriedade mostrada em cada opção)
            var categoriasSelectList = new SelectList(categorias, nameof(CategoriaModel.IdCategoria), nameof(CategoriaModel.Nome));
            ViewBag.Categorias = categoriasSelectList;
            if (id.HasValue)
            {
                var produto = await _context.Produtos.FindAsync(id);
                if (produto == null)
                {
                    return NotFound();
                }
                return View(produto);
            }
            return View(new ProdutoModel());
        }

        private bool ProdutoExiste(int id)
        {
            return _context.Produtos.Any(c => c.IdProduto == id);    
        }

        [HttpPost]
        /* Indica que o produto será alimentado pelo formulário */
        public async Task<IActionResult> Cadastrar( int? id, [FromForm] ProdutoModel produto)
        {
            //
            if (produto.IdProduto != 0)
                id = produto.IdProduto;
            /* São aplicadas todas as restrições definidas, entenda-se, Se os critérios estabelecidos forem satisfeitos */
            if (ModelState.IsValid)
            {
                //Se o id existe é sinal que é uma alteração
                if (id.HasValue)
                {
                    //Busca pelo id e retorna verdadeiro se o encontra
                    if (ProdutoExiste(id.Value))
                    {
                        //atualiza a produto, salva as alterações
                        _context.Update(produto);

                        //define se ocorreu alguma mudança
                        if (await _context.SaveChangesAsync() > 0)
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Produto alterado com sucesso.");
                        }
                        else
                        {
                            TempData["mensagem"] = MensagemModel.Serializar("Erro ao alterar produto.", TipoMensagem.Erro);
                        }
                    }                    
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado.", TipoMensagem.Erro);
                    }
                }
                else
                {
                    //Caso o id não exista, ocorre uma inclusão
                    _context.Add(produto);
                    // _context.Produtos.Add(produto);
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Produto cadastrado com sucesso.");
                    }
                    else
                    {
                        TempData["mensagem"] = MensagemModel.Serializar("Erro ao cadastrar produto.", TipoMensagem.Erro);
                    }
                    //return RedirectToAction(nameof(Index));
                    
                }
                //return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }

            /* Inpeção de erros para o ModelState */
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Erro: {modelError.ErrorMessage}");
            }
            //Faz com que permaneça na view do formulário
            return View(produto);                
        }

        [HttpGet]
        public async Task<IActionResult> Excluir ( int? id)
        {
            if(!id.HasValue)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não informado", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado.", TipoMensagem.Erro);
                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }
        [HttpPost]
        public async Task<IActionResult> Excluir (int id)
        {
            var produto = await _context.Produtos.FindAsync(id); 
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                if (await _context.SaveChangesAsync() > 0)
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Produto excluído com sucesso.");
                }
                else
                {
                    TempData["mensagem"] = MensagemModel.Serializar("Não foi possível excluir o produto.", TipoMensagem.Erro);                                   
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["mensagem"] = MensagemModel.Serializar("Produto não encontrado.", TipoMensagem.Erro); 
                return RedirectToAction(nameof(Index));                                  
            }
        }
    }    
}