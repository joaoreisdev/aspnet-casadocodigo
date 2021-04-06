using CasaDoCodigo.Models;
using CasaDoCodigo.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace CasaDoCodigo
{
    //Classe de serviços de dados
    class DataService : IDataService
    {
        private readonly ApplicationContext contexto;
        private readonly IProdutoRepository produtoRepository;
        
        // Ao invés de iniciar o contexto na classe utilizamos um conceito de injeção de dependencia para gera-lo.
        public DataService(ApplicationContext contexto, IProdutoRepository produtoRepository)
        {
            this.contexto = contexto;
            this.produtoRepository = produtoRepository;
        }

        public void inicializaDB()
        {
            //Garante que ele foi gerado 
            contexto.Database.Migrate();

            List<Livro> livros = GetLivros();

            //Desabilitei o salvar para não salvar toda os mesmos produtos
            produtoRepository.SaveProdutos(livros);

        }

        private static List<Livro> GetLivros()
        {
            var json = File.ReadAllText("livros.json");
            var livros = JsonConvert.DeserializeObject<List<Livro>>(json);
            return livros;
        }
    }



}
