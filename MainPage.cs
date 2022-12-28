namespace ExportPictures
{
    public partial class MainPage : ContentPage
    {
        Label title;
        Button insert;
        Button retrieve;

        public MainPage()
        {
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

        private void OnInsertClick(object? sender, EventArgs e)
        {
            DisplayAlert("Alert", "Insert", "OK");
        }

        private void OnRetrieveClick(object? sender, EventArgs e)
        {
            DisplayAlert("Alert", "Retrieve", "OK");
        }
    }
}
