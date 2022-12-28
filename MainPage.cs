﻿using System.Reflection;
using System.Reflection.Metadata;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace ExportPictures
{
    public partial class MainPage : ContentPage
    {
        MySqlConnection mySqlConnection;

        Label title;
        Button insert;
        Button retrieve;

        public MainPage()
        {
            // TODO: Retirar usuário e senha hardcoded no aplicativo
            mySqlConnection = new MySqlConnection("server=localhost; user id=root; password=p@ssw0rd;");
            mySqlConnection.Open();

            title = new Label()
            {
                Style = AppResource<Style>("MauiLabel"),
                Text = "Welcome to .NET Multi-platform App UI",
            }.FontSize(18).CenterHorizontal().SemanticDesc("Welcome to dot net Multi platform App U I").SemanticHeading(SemanticHeadingLevel.Level1);
            insert = new Button()
            {
                Style = AppResource<Style>("PrimaryAction"),
                Text = "Inserir",
                WidthRequest = 150
            }.CenterHorizontal().Invoke(btn => btn.Clicked += OnInsertClick);
            retrieve = new Button()
            {
                Style = AppResource<Style>("PrimaryAction"),
                Text = "Recuperar",
                WidthRequest = 150
            }.CenterHorizontal().Invoke(btn => btn.Clicked += OnRetrieveClick);
 
            Build();
        }

        private void Build()
        {
            Content = new ScrollView()
            {
                Content = new StackLayout()
                {
                    Spacing = 25,
                    Children =
                    {
                        title,
                        new HorizontalStackLayout()
                        {
                            Children = { insert, retrieve },
                            Spacing = 10,
                        }.CenterHorizontal(),
                        new Image()
                        {
                            Source = "dotnet_bot.png",
                        }.Height(310).Width(250).CenterHorizontal().SemanticDesc("Cute dot net bot waving hi to you!"),
                    }
                }.Padding(30)
            };
        }

        // Para fazer o upload para o banco de dados é necessário aumentar o tamanho do packet no MySQL
        // SET GLOBAL max_allowed_packet = 850741824
        private void OnInsertClick(object? sender, EventArgs e)
        {
            /*
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() != DialogResult.OK) return;
            */

            String filename = "1";   // Path.GetFileNameWithoutExtension(fileDialog.FileName);
            Byte[] fileContents = new Byte[] {}; // File.ReadAllBytes(fileDialog.FileName);
            String extension = "jpg"; // Path.GetExtension(fileDialog.FileName); extension = extension.Trim('.');

            String query = "UPDATE `commercedb`.`produto` SET `foto` = '" + Convert.ToBase64String(fileContents) + "', `formatoImagem` = 'image/" + extension + ";base64' WHERE id=" + filename;
            MySqlCommand command = new MySqlCommand(query, mySqlConnection);
            int rowsAffected = command.ExecuteNonQuery();

            DisplayAlert("Informação", rowsAffected + " registro(s) foram inseridos no Banco de Dados", "OK");
        }

        private List<Produto> GetProducts(String filter)
        {
            List<Produto> productList = new List<Produto>();

            String query = "SELECT * FROM `commercedb`.`produto`";
            if (!String.IsNullOrEmpty(filter)) query += " WHERE " + filter;
            MySqlCommand command = new MySqlCommand(query, this.mySqlConnection);
            MySqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                Produto product = new Produto();
                product.nome      = (String)dataReader["nome"];
                product.descricao = (String)dataReader["descricao"];
                product.preco     = (Decimal)dataReader["preco"];
                product.foto      = dataReader["foto"] is DBNull ? "" : (String)dataReader["foto"];
                product.formatoImagem = dataReader["formatoImagem"] is DBNull ? "" : (String)dataReader["formatoImagem"];

                productList.Add(product);
            }
            dataReader.Close();

            return productList;
        }

        private void OnRetrieveClick(object? sender, EventArgs e)
        {
            List<Produto> productList = GetProducts("");
            foreach (Produto product in productList)
            {
                if (String.IsNullOrEmpty(product.formatoImagem)) continue; // skip this one

                String filename = product.nome + '.' + product.formatoImagem;
                // Char[] blobContents = Encoding.UTF8.GetChars(document.arquivo);
                // Byte[] fileContents = Convert.FromBase64CharArray(blobContents, 0, blobContents.Length);
                Byte[] fileContents = Convert.FromBase64String(product.foto);
                FileStream fileStream = new FileStream(filename, FileMode.CreateNew);
                fileStream.Write(fileContents, 0, fileContents.Length);
                fileStream.Flush();
            }

            String outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + " ou " + Directory.GetCurrentDirectory();
            DisplayAlert("Informação", "Arquivos salvos no diretório " + outputDir, "OK");
        }
    }
}
