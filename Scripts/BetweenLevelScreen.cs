using Godot;

public class BetweenLevelScreen : Control
{
    private static BetweenLevelScreen _instance; 
    
    private Label _dayText;
    private Label _quotaText;
    private Label _rankText;
    private Button _button; 
    
    public override void _Ready()
    {
        _instance = this; 
        
        _dayText = GetNode<Label>("DayText");
        _quotaText = GetNode<Label>("QuotaText");
        _rankText = GetNode<Label>("RankText");
        _button = GetNode<Button>("AdvanceButton");

        _button.Connect("pressed", this, nameof(OnAdvanceButton));
    }

    public void OnAdvanceButton()
    {
        Fader.FadeOut("hide_stat_menu");
    }

    public static void SetText(string dayText, string bodyText, string rankText, string buttonText)
    {
        _instance._dayText.Text = dayText;
        _instance._quotaText.Text = bodyText;
        _instance._rankText.Text = rankText;
        _instance._button.Text = buttonText;
    }

    public void OnFadeApex(string callback)
    {
        switch (callback)
        {
            case "show_stat_menu":
                Visible = true; 
                break;
            
            case "hide_stat_menu":
                Visible = false;
                break;
        }
    }
}