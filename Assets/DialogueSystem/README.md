# Dialogue System Editor Unity Package

This package offers a Unity editor window to create and manage dialogue systems.  
It is designed to be quickly design simple dialogue systems for games inside Unity itself.

# Installation
1. Open the Unity Package Manager `(Window -> Package Manager)`
5. Click on the "+" button and select `"Add package from git URL..."`
6. Paste the following URL: `git+https://github.com/AdriKat2022/DialogueSystemUnityPackage.git`
7. Click on "Install"
8. Enjoy!

> If you wish to install a particular version, you can append `#[tagname]` at the end of the URL.  
For example, to install the version tagged by `v1.0.2`, use the url:
`git+https://github.com/AdriKat2022/DialogueSystemUnityPackage.git#v1.0.2`

# Usage
1. Open the Dialogue System Editor window `(Window -> Dialogue System Editor)`
2. Create nodes by right-clicking
3. Make dialogues!

Save with the "Save" button and load with the "Load" button.  
Warning: There is no confirmation dialog yet when saving, loading or quitting without saving so be careful to save each time!

# Features
- [Single Nodes](#single-nodes) (basic nodes)
- [Multiple Nodes](#multiple-nodes) (nodes with choices)
- [Conditional Nodes](#conditional-nodes) (nodes with conditions)
- [Authors](#authors) (usable by any node that can displays text)

> All nodes that display text can have an author. See the [Authors](#authors) section below.

## Single Nodes
Single nodes are the most basic nodes.  
They contain a single text with no possibility whatsoever and either end or continue to another node.

## Multiple Nodes
Multiple nodes are branch nodes according to the player's choice.  
They contain a text and a variable list of options whose length is adjustable.  
Each option bears a name and can lead to a specific node.

## Conditional Nodes
Conditional nodes are branch nodes according to a condition or list of conditions.  
This node doesn't appear at any time in the dialogue and is purely logical, as the story automatically continues to the appropriate node. 
They only have two outputs: one for when the condition is true and one for when it is false.  

### Condition mode
- **All**: All conditions of the list must be true to go to the true output.
- **Any**: At least one condition of the list must be true to go to the true output.

### Condition types
Conditions are currently supported for the following types and their respective operators:
- **Boolean**: Is, And, Or, Xor
- **Integer**: Equal, NotEqual, Greater, GreaterOrEqual, Less, LesserOrEqual
- **String**: Equal, NotEqual, Contains, StartsWith, EndsWith

### Condition Variables
Conditions are great, but they need values to be compared to!  
For this, the dynamic variables used for the conditions are stored in a static dictionary at runtime, using strings to localize them.  

> **STRINGS? REALLY? Yes, strings.**

However **DOT NOT FEAR**, as the editor provides a very useful way to manage these values in a user-friendly way!  
I decided to make it **TYPE-SAFE**, meaning you only have to write the name of the variable once and then you can select it from a dropdown list everywhere else.  
Pretty neat, huh?

This is how it works:
1. **Create a new DialogueVariableNames scriptable object `(Assets -> Create -> Dialogue System -> Variable Names Condition Container)`**
2. **Define your variables in there**  

Yep that's it! Just don't forget to use a Condition Initializer `(GameObject->Dialogue System->Variable Condition Initializer)` in your scene (or add the Condition Initializer component directly in any game object active at the beginning, you choose!).

You only need one at the very beginning of your game. It will initialize the dictionary with the values you set in the scriptable object that you give it. Then you can even tell it to disappear once it has done its job!

> **Note**: Variables can only be tested against **hard coded values** for now.

#### Condition variables in CODE
Of course, the variables you declared in the scriptable object can be accessed in your code! Yeah otherwise *static* variables would be pretty useless, right?

Okay remember when I said it was type-safe? Well, I kinda lied.  
You can't access the variables directly in code, but you can access them through the `DialogueVariable` static class with the proper variable key.

```csharp
using AdriKat.DialogueSystem.Variables;
using UnityEngine;

public class MyScript : MonoBehaviour
{
    private void Start()
    {
        // Get the value of a variable
        int myInt = (int)DialogueVariable.GetInt("MyInt");
        string myString = (string)DialogueVariable.GetString("MyString");
        bool myBool = (bool)DialogueVariable.GetBool("MyBool");

        // Set the value of a variable
        DialogueVariable.SetInt("MyInt", 42);
        DialogueVariable.SetString("MyString", "Hello World!");
        DialogueVariable.SetBool("MyBool", true);

        // Returns NULL is the variable doesn't exist
        int? myInt2 = DialogueVariable.GetInt("MyInt2");

        // Creates the variable if it doesn't exist
        DialogueVariable.SetInt("MyInt2", 42);
    }
}
```

I'm sorry for the inconvenience, I'm working on it.  
Maybe generating at compilation or on prompt a enum with the variable names for instance?  
I don't know yet, but I'll find a way to make it better!  

## Authors
Authors are a way to add a name to the text of a node.  
They are not mandatory, but they can be useful to identify who is speaking.  
You can add any author to any node that displays text.  
All the author's info is stored in the `DialogueSO.AuthorDecorator.AuthorData` field of the scriptable object. It should be guaranteed to be not null if `DialogueSO.HasAuthor` is true in normal usage.

### Creation and localization
**To create an author:**
1. Go to the folder `Assets/Resources/DialogueSystem/Authors` or create it if it doesn't exist.
2. Right Click `New -> Dialogue System -> Dialogue Author` and fill in the fields.  

Alternatively, a button `Create Author` is shown if no author exists yet and the toggle `Has an author` of a dialogue node is checked. It will create a new author in the `Assets/Resources/DialogueSystem/Authors` folder.

**To select an existing author:**
- Simply select the author you want in the dropdown list of the node.

> If the author you seek is not in the list, check that the author is in the `Assets/Resources/DialogueSystem/Authors` folder. **The name that appears in the dropdown list is the name of the filename, not the author** (planning to change this in the future).

- **Enable or disable mugshot:** Once an author is selected, you can then choose if you want to display the mugshot of the author or not.

> If no mugshot is available for the selected author, the mugshot will be null, the node will show you a warning and this checkbox will be disabled.

- **Select the emotion:** If the mugshot is displayed, choose the emotion of the author in the dropdown list. This will change the mugshot displayed in the dialogue box. The mugshot will be null if there is no mugshot for the selected emotion. The currently selected emotion sprite is previewed in the node.

### Composition of an Author
An author is currently composed of:
- `string` **Name:** The name of the author. This is what could be displayed as the dialogue box's title.
- `Dict<string, Sprite>` **Sprites:** Dictionnary of sprites of the author that serves as different mugshot expressions. Each identified by strings.

> Authors are currently **immutable** from the nodes. This means those can only be modified from the author scriptable object itself.  
This is to prevent any confusion when using the same author in multiple nodes.

> **TO NOT BE MISTAKEN WITH A [`AuthorDecorator`](#composition-of-an-authordecorator) THAT ON THE CONTRARY IS MUTABLE.**

### Composition of an AuthorDecorator
Present in the `DialogueSO` class, the `AuthorDecorator` is a way to add an author to a node.  
It is a simple class that contains the following fields:
An authorDecorator is currently composed of:
- `bool` **HasAuthor:** Whether the node has an author or not. This is used to display the author name in the dialogue box. Usually guarentees a non-null author if this is true.
- [`Author`](#composition-of-an-author) **Author:** The author associated to this dialogue.
- `bool` **HasMugshot:** Whether the node wants to display a mugshot or not. This is used to display the mugshot in the dialogue box. Does **NOT** guarentees a non-null mugshot even if this is true.
- `string` **Emotion:** Name of the emotion of the author to display. This is used to display the mugshot in the dialogue box. This is a string that corresponds to the name of the sprite in the list of sprites of the author.

# Future Features
- [ ] Confirmation dialog when saving, loading or quitting without saving
- [ ] More Author data (voice lines, color or more)
- [ ] Improve author-usage clarity (name of the author instead of the filename, better documentation and ready-to-call functions for basic operations)
- [ ] Overridable options for easy title and mugshot settings (for example, if you want to set the title and mugshot of the dialogue box in the node directly, you could do it easily without having to go through the author scriptable object)
- [ ] Add a way to create a new author directly from the node (currently, you have to create it in the `Assets/Resources/DialogueSystem/Authors` folder with a right click - considering a button in the node itself or a whole new window for author creation and management)

# Known Issues
I'm not aware of any major issue, but I'm sure there are some!