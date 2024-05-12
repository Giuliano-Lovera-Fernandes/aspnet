using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EstoqueWeb.Models
{
    public class EstoqueWebContext : DbContext
    {  
        /* Necessário informar qual coleções de objetos vai ter */
        public DbSet<CategoriaModel> Categorias { get; set; } 
        public DbSet<ProdutoModel> Produtos { get; set; }

        public DbSet<ClienteModel> Clientes { get; set; }

        public DbSet<PedidoModel> Pedidos { get; set; }

        public DbSet<ItemPedidoModel> ItensPedidos { get; set; }

        public EstoqueWebContext(DbContextOptions<EstoqueWebContext> options)
            : base(options)
        {            
        }

        /* Ao ser criado deve ser adicionado ao contexto de injeção de dependência, ou melhor ao
        repositório de objetos a ser injetados através da injeção de dependência */


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoriaModel>().ToTable("Categoria");
            modelBuilder.Entity<ProdutoModel>().ToTable("Produto");
            modelBuilder.Entity<ClienteModel>()
                .OwnsMany(c => c.Enderecos, e =>
                {
                    e.WithOwner().HasForeignKey("IdUsuario");
                    e.HasKey("IdUsuario", "IdEndereco");
                    e.Property<int>("IdUsuario").ValueGeneratedNever();
                    e.Property<int>("IdEndereco").ValueGeneratedNever();
                });


            /* Reparar que nessa foi passada uma função Sql (porque quem vai resolver é o banco de dados), enquanto 
            no debaixo, apenas um valor */
            modelBuilder.Entity<UsuarioModel>().Property(u => u.DataCadastro)
                .HasDefaultValueSql("datetime('now', 'localtime', 'start of day')")
                /* pega a data de hoje no formato da hora local e a hora inícial do dia é às zero horas */
                /* Não será possível salvar dados em DataCadastro - valor default nunca modificado */

                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<ProdutoModel>()
                .Property(p => p.Estoque).HasDefaultValue(0); 
                /* possui um endereço de entrega */
            modelBuilder.Entity<PedidoModel>()
                .OwnsOne(p => p.EnderecoEntrega, e =>
                {
                    e.Ignore(e => e.IdEndereco);
                    e.Ignore(e => e.Selecionado);
                    /* Endereço fica armazenado no pedido*/
                    e.ToTable("Pedido");
                });
            modelBuilder.Entity<ItemPedidoModel>()
                .HasKey(ip => new {ip.IdPedido, ip.IdProduto});
        }
    }
}