﻿using System.Windows.Input;

namespace TemplateMAUI.Controls
{
    /// <summary>
    /// The IButton interface defines the properties that a button control should implement. 
    /// These properties include the content, visual appearance, and behavior of the button, such as the background color, border, text color, font settings, and the command to be executed when the button is pressed.
    /// </summary>
    interface IButton
    {
        object Content { get; }
        Brush Background { get; }
        Brush BorderBrush { get; }
        double BorderThickness { get; }
        CornerRadius CornerRadius { get; }
        Color TextColor { get; }
        double FontSize { get; }
        string FontFamily { get; }
        FontAttributes FontAttributes { get; }
        Color RippleColor { get; }
        ICommand Command { get; }
        object CommandParameter { get; }
    }
}