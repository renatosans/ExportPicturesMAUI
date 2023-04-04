using System.Reflection;
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
                Text = "Export Pictures (.NET MAUI)",
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
                            Source = "catalogo_produtos.png",
                        }.Height(400).Width(600).CenterHorizontal().SemanticDesc("Catalogo"),
                    }
                }.Padding(30)
            };
        }

        // Para fazer o upload para o banco de dados é necessário aumentar o tamanho do packet no MySQL
        // SET GLOBAL max_allowed_packet = 850741824
        private async void OnInsertClick(object? sender, EventArgs e)
        {
            var fileResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Pick Image Please",
                FileTypes = FilePickerFileType.Images
            });

            if (fileResult == null) return;

            String filename = Path.GetFileNameWithoutExtension(fileResult.FileName);
            Byte[] fileContents = File.ReadAllBytes(fileResult.FullPath);
            String extension = Path.GetExtension(fileResult.FileName); extension = extension.Trim('.');

            String query = "INSERT INTO `commercedb`.`produto`(nome, preco, foto, formatoImagem) VALUES('" + filename + "', 1.99, '" + Convert.ToBase64String(fileContents) + "', 'image/" + extension + ";base64')";
            MySqlCommand command = new MySqlCommand(query, mySqlConnection);
            int rowsAffected = 0;
            try
            {
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                DisplayAlert("Erro", error.Message, "OK");
            }

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
                product.descricao = dataReader["descricao"] is DBNull ? "" : (String)dataReader["descricao"];
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
            List<Produto> productList = new List<Produto>() { };

            String outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\img\";
            Directory.CreateDirectory(outputDir);
            try
            {
                productList = GetProducts("");
            }
            catch (Exception error)
            {
                DisplayAlert("Erro", error.Message, "OK");
            }

            int filesWritten = 0;
            foreach (Produto product in productList)
            {
                String filename = product.nome + '.' + product.formatoImagem.Replace(@"image/", "").Replace(@";base64", "");

                if (String.IsNullOrEmpty(product.formatoImagem)) continue; // skip this one
                if (File.Exists(outputDir + filename))
                {
                    DisplayAlert("Informação", "O arquivo " + filename + " já existe", "OK");
                    continue; // skip this one
                }

                // Char[] blobContents = Encoding.UTF8.GetChars(document.arquivo);
                // Byte[] fileContents = Convert.FromBase64CharArray(blobContents, 0, blobContents.Length);
                Byte[] fileContents = Convert.FromBase64String(product.foto);
                FileStream fileStream = new FileStream(outputDir + filename, FileMode.CreateNew);
                fileStream.Write(fileContents, 0, fileContents.Length);
                fileStream.Flush();
                filesWritten++;
            }
 
            if (filesWritten == 0)
            {
                DisplayAlert("Informação", "Nenhum arquivo gravado em disco ", "OK");
                return;
            }
            DisplayAlert("Informação", "Arquivos salvos no diretório " + outputDir, "OK");
        }
    }
}
