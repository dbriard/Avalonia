﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using Avalonia.Controls.Embedding;
using Avalonia.Win32.Interop;
using WinFormsControl = System.Windows.Forms.Control;
using AvControl = Avalonia.Controls.Control;

namespace Avalonia.Win32.Interoperability
{
    /// <summary>
    /// An element that allows you to host a Avalonia control on a Windows Forms page.
    /// </summary>
    [ToolboxItem(true)]
    public class WinFormsAvaloniaControlHost : WinFormsControl
    {
        private readonly EmbeddableControlRoot _root = new();

        private IntPtr WindowHandle => _root?.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormsAvaloniaControlHost"/> class.
        /// </summary>
        public WinFormsAvaloniaControlHost()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            UnmanagedMethods.SetParent(WindowHandle, Handle);
            _root.Prepare();
            if (_root.IsFocused)
                _root.FocusManager?.ClearFocus();
            _root.GotFocus += RootGotFocus;

            FixPosition();
        }

        /// <summary>
        /// Gets or sets the Avalonia control hosted by the <see cref="WinFormsAvaloniaControlHost"/> element.
        /// </summary>
        public AvControl? Content
        {
            get => (AvControl?)_root.Content;
            set => _root.Content = value;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _root.Dispose();
            base.Dispose(disposing);
        }

        private void RootGotFocus(object? sender, Interactivity.RoutedEventArgs e)
        {
            UnmanagedMethods.SetFocus(WindowHandle);
        }

        /// <inheritdoc />
        protected override void OnGotFocus(EventArgs e)
        {
            var handle = WindowHandle;
            if (handle != default)
                UnmanagedMethods.SetFocus(handle);
        }
        
        private void FixPosition()
        {
            var handle = WindowHandle;
            if (handle != default && Width > 0 && Height > 0)
                UnmanagedMethods.MoveWindow(handle, 0, 0, Width, Height, true);
        }
        
        /// <inheritdoc />
        protected override void OnResize(EventArgs e)
        {
            FixPosition();
            base.OnResize(e);
        }

        /// <inheritdoc />
        protected override void OnPaint(PaintEventArgs e)
        {

        }
    }
}
