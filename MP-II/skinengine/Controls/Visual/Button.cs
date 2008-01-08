#region Copyright (C) 2007 Team MediaPortal

/*
    Copyright (C) 2007 Team MediaPortal
    http://www.team-mediaportal.com
 
    This file is part of MediaPortal II

    MediaPortal II is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal II is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal II.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using MediaPortal.Core.Properties;
using SkinEngine.Controls.Visuals.Styles;
using MediaPortal.Core.InputManager;
using MediaPortal.Core.Collections;
using SkinEngine;
using SkinEngine.Controls.Bindings;

namespace SkinEngine.Controls.Visuals
{
  public class Button : Border
  {
    Property _templateProperty;
    Property _styleProperty;
    Property _isPressedProperty;

    Property _commandParameter;
    Command _command;
    Command _contextMenuCommand;
    Property _contextMenuCommandParameterProperty;

    public Button()
    {
      Init();
    }

    public Button(Button b)
      : base(b)
    {
      Init();
      IsPressed = b.IsPressed;
      Template = (UIElement)b.Template.Clone();
      Style = b.Style;

      Command = b.Command;
      CommandParameter = b._commandParameter;

      ContextMenuCommand = b.ContextMenuCommand;
      ContextMenuCommandParameter = b.ContextMenuCommandParameter;
    }

    public override object Clone()
    {
      return new Button(this);
    }

    void Init()
    {
      _templateProperty = new Property(null);
      _styleProperty = new Property(null);
      _isPressedProperty = new Property(false);
      _commandParameter = new Property(null);
      _command = null;
      _contextMenuCommandParameterProperty = new Property(null);
      _contextMenuCommand = null;
      Focusable = true;
      _styleProperty.Attach(new PropertyChangedHandler(OnStyleChanged));
    }

    void OnStyleChanged(Property property)
    {
      Style.Set(this);
      this.Template.VisualParent = this;
      Invalidate();
    }

    /// <summary>
    /// Gets or sets the is pressed.
    /// </summary>
    /// <value>The is pressed.</value>
    public Property IsPressedProperty
    {
      get
      {
        return _isPressedProperty;
      }
      set
      {
        _isPressedProperty = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is pressed.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is pressed; otherwise, <c>false</c>.
    /// </value>
    public bool IsPressed
    {
      get
      {
        return (bool)_isPressedProperty.GetValue();
      }
      set
      {
        _isPressedProperty.SetValue(value);
      }
    }

    /// <summary>
    /// Gets or sets the control template property.
    /// </summary>
    /// <value>The control template property.</value>
    public Property TemplateProperty
    {
      get
      {
        return _templateProperty;
      }
      set
      {
        _templateProperty = value;
      }
    }

    /// <summary>
    /// Gets or sets the control template.
    /// </summary>
    /// <value>The control template.</value>
    public UIElement Template
    {
      get
      {
        return _templateProperty.GetValue() as UIElement;
      }
      set
      {
        _templateProperty.SetValue(value);
      }
    }


    /// <summary>
    /// Gets or sets the control style property.
    /// </summary>
    /// <value>The control style property.</value>
    public Property StyleProperty
    {
      get
      {
        return _styleProperty;
      }
      set
      {
        _styleProperty = value;
      }
    }

    /// <summary>
    /// Gets or sets the control style.
    /// </summary>
    /// <value>The control style.</value>
    public Style Style
    {
      get
      {
        return _styleProperty.GetValue() as Style;
      }
      set
      {
        _styleProperty.SetValue(value);
      }
    }


    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>The command.</value>
    public Command Command
    {
      get
      {
        return _command;
      }
      set
      {
        _command = value;
      }
    }
    /// <summary>
    /// Gets or sets the command parameter property.
    /// </summary>
    /// <value>The command parameter property.</value>
    public Property CommandParameterProperty
    {
      get
      {
        return _commandParameter;
      }
      set
      {
        _commandParameter = value;
      }
    }

    /// <summary>
    /// Gets or sets the control style.
    /// </summary>
    /// <value>The control style.</value>
    public object CommandParameter
    {
      get
      {
        return _commandParameter.GetValue();
      }
      set
      {
        _commandParameter.SetValue(value);
      }
    }

    /// <summary>
    /// Gets or sets the context menu command.
    /// </summary>
    /// <value>The context menu command.</value>
    public Command ContextMenuCommand
    {
      get
      {
        return _contextMenuCommand;
      }
      set
      {
        _contextMenuCommand = value;
      }
    }

    /// <summary>
    /// Gets or sets the context menu command parameter property.
    /// </summary>
    /// <value>The context menu command parameter property.</value>
    public Property ContextMenuCommandParameterProperty
    {
      get
      {
        return _contextMenuCommandParameterProperty;
      }
      set
      {
        _contextMenuCommandParameterProperty = value;
      }
    }

    /// <summary>
    /// Gets or sets the context menu command parameter.
    /// </summary>
    /// <value>The context menu command parameter.</value>
    public object ContextMenuCommandParameter
    {
      get
      {
        return _contextMenuCommandParameterProperty.GetValue();
      }
      set
      {
        _contextMenuCommandParameterProperty.SetValue(value);
      }
    }


    /// <summary>
    /// measures the size in layout required for child elements and determines a size for the FrameworkElement-derived class.
    /// </summary>
    /// <param name="availableSize">The available size that this element can give to child elements.</param>
    public override void Measure(System.Drawing.Size availableSize)
    {
      if (Template == null)
      {
        _desiredSize = new System.Drawing.Size((int)Width, (int)Height);
        if (Width <= 0)
          _desiredSize.Width = ((int)100) - (int)(Margin.X + Margin.W);
        if (Height <= 0)
          _desiredSize.Height = ((int)32) - (int)(Margin.Y + Margin.Z);

        _desiredSize.Width += (int)(Margin.X + Margin.W);
        _desiredSize.Height += (int)(Margin.Y + Margin.Z);
        _availableSize = new System.Drawing.Size(availableSize.Width, availableSize.Height);
        return;
      }

      _desiredSize = new System.Drawing.Size((int)Width, (int)Height);
      if (Width <= 0)
        _desiredSize.Width = (int)availableSize.Width - (int)(Margin.X + Margin.W);
      if (Height <= 0)
        _desiredSize.Height = (int)availableSize.Height - (int)(Margin.Y + Margin.Z);

      Template.Measure(_desiredSize);

      if (Width <= 0)
        _desiredSize.Width = Template.DesiredSize.Width;

      if (Height <= 0)
        _desiredSize.Height = Template.DesiredSize.Height;

      _desiredSize.Width += (int)(Margin.X + Margin.W);
      _desiredSize.Height += (int)(Margin.Y + Margin.Z);
      _availableSize = new System.Drawing.Size(availableSize.Width, availableSize.Height);
    }

    /// <summary>
    /// Arranges the UI element
    /// and positions it in the finalrect
    /// </summary>
    /// <param name="finalRect">The final size that the parent computes for the child element</param>
    public override void Arrange(System.Drawing.Rectangle finalRect)
    {
      _finalRect = new System.Drawing.Rectangle(finalRect.Location, finalRect.Size);
      System.Drawing.Rectangle layoutRect = new System.Drawing.Rectangle(finalRect.X, finalRect.Y, finalRect.Width, finalRect.Height);
      layoutRect.X += (int)(Margin.X);
      layoutRect.Y += (int)(Margin.Y);
      layoutRect.Width -= (int)(Margin.X + Margin.W);
      layoutRect.Height -= (int)(Margin.Y + Margin.Z);
      ActualPosition = new Microsoft.DirectX.Vector3(layoutRect.Location.X, layoutRect.Location.Y, 1.0f); ;
      ActualWidth = layoutRect.Width;
      ActualHeight = layoutRect.Height;
      if (Template != null)
      {
        Template.Arrange(layoutRect);
        ActualPosition = Template.ActualPosition;
        ActualWidth = ((FrameworkElement)Template).ActualWidth;
        ActualHeight = ((FrameworkElement)Template).ActualHeight;
      }

      if (!IsArrangeValid)
      {
        IsArrangeValid = true;
        InitializeBindings();
        InitializeTriggers();
      }
    }

    /// <summary>
    /// Renders the visual
    /// </summary>
    public override void DoRender()
    {
      base.DoRender();
      if (Template != null)
      {
        Template.DoRender();
      }
    }

    /// <summary>
    /// Fires an event.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    public override void FireEvent(string eventName)
    {
      if (Template != null)
      {
        Template.FireEvent(eventName);
      }
      base.FireEvent(eventName);
    }

    /// <summary>
    /// Find the element with name
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public override UIElement FindElement(string name)
    {
      if (Template != null)
      {
        UIElement o = Template.FindElement(name);
        if (o != null) return o;
      }
      return base.FindElement(name);
    }

    public override UIElement FindElementType(Type t)
    {
      if (Template != null)
      {
        UIElement o = Template.FindElementType(t);
        if (o != null) return o;
      }
      return base.FindElementType(t);
    }

    public override UIElement FindItemsHost()
    {
      if (Template != null)
      {
        UIElement o = Template.FindItemsHost();
        if (o != null) return o;
      }
      return base.FindItemsHost(); ;
    }

    /// <summary>
    /// Finds the focused item.
    /// </summary>
    /// <returns></returns>
    public override UIElement FindFocusedItem()
    {
      if (HasFocus) return this;
      if (Template != null)
      {
        UIElement o = Template.FindFocusedItem();
        if (o != null) return o;
      }
      return null;
    }

    /// <summary>
    /// Called when [mouse move].
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    public override void OnMouseMove(float x, float y)
    {
      if (Template != null)
      {
        Template.OnMouseMove(x, y);
      }
      base.OnMouseMove(x, y);
    }

    /// <summary>
    /// Animates any timelines for this uielement.
    /// </summary>
    public override void Animate()
    {
      if (Template != null)
      {
        Template.Animate();
      }
      base.Animate();
    }

    /// <summary>
    /// Gets or sets a value indicating whether this uielement has focus.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this uielement has focus; otherwise, <c>false</c>.
    /// </value>
    public override bool HasFocus
    {
      get
      {
        return base.HasFocus;
      }
      set
      {
        base.HasFocus = value;
        if (value == false)
          IsPressed = false;
        Trace.WriteLine(String.Format("{0} focus:{1}", Name, value));
      }
    }

    /// <summary>
    /// Handles keypresses
    /// </summary>
    /// <param name="key">The key.</param>
    public override void OnKeyPressed(ref Key key)
    {
      if (!HasFocus) return;
      if (key == MediaPortal.Core.InputManager.Key.None) return;
      if (key == MediaPortal.Core.InputManager.Key.Enter)
      {
        IsPressed = true;
      }
      if (key == MediaPortal.Core.InputManager.Key.Enter)
      {
        if (Command != null)
        {
          Command.Method.Invoke(Command.Object, new object[] { CommandParameter });
        }
        if (Context is ListItem)
        {
          ListItem listItem = Context as ListItem;
          if (listItem.Command != null)
          {
            listItem.Command.Execute(listItem.CommandParameter);
          }
        }
      }

      if (key == MediaPortal.Core.InputManager.Key.ContextMenu)
      {
        if (ContextMenuCommand != null)
        {
          ContextMenuCommand.Method.Invoke(ContextMenuCommand.Object, new object[] { ContextMenuCommandParameter });
        }
      }

      UIElement cntl = FocusManager.PredictFocus(this, ref key);
      if (cntl != null)
      {
        HasFocus = false;
        cntl.HasFocus = true;
        key = MediaPortal.Core.InputManager.Key.None;
      }
    }
  }
}
