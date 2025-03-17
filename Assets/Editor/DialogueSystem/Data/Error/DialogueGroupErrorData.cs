using System.Collections.Generic;

public class DialogueGroupErrorData
{
    public DialogueErrorData ErrorData { get; set; }
    public List<DialogueGroup> Groups { get; set; }

    public DialogueGroupErrorData()
    {
        ErrorData = new DialogueErrorData();
        Groups = new List<DialogueGroup>();
    }
}
