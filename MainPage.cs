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

        private void OnRetrieveClick(object? sender, EventArgs e)
        {
            DisplayAlert("Alert", "Retrieve", "OK");
        }
    }
}
