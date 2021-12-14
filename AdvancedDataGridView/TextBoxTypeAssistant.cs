#region License
// Advanced DataGridView
//
// Copyright (c), 2014 Davide Gironi <davide.gironi@gmail.com>
// Original work Copyright (c), 2013 Zuby <zuby@me.com>
//
// Please refer to LICENSE file for licensing information.
#endregion

using System;
using System.Windows.Forms;

namespace Zuby.ADGV
{
    internal class TextBoxTypeAssistant
    {
        #region class fields

        private readonly System.Threading.Timer _delayTimer;
        private bool _enabled = true;

        #endregion

        #region public properties

        /// <summary>
        /// Get or set the delay.
        /// </summary>
        public int DelayMilliSeconds { get; set; }

        /// <summary>
        /// Enable state of the assistent.
        /// Setting enabled to false will cancel any ongoing delay.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set 
            {   
                _enabled = value;
                if(!_enabled)
                    _delayTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
        }

        #endregion

        #region public events

        public event EventHandler TextChanged;

        #endregion

        #region constructors

        /// <summary>
        /// TextBoxTypeAssistant constructor
        /// </summary>
        /// <param name="textbox"></param>
        /// <param name="delayMilliSeconds"></param>
        public TextBoxTypeAssistant(TextBox textbox, int delayMilliSeconds)
        {
            DelayMilliSeconds = delayMilliSeconds;

            _delayTimer = new System.Threading.Timer(p =>
            {
                textbox.Invoke(
                    new MethodInvoker(() =>
                        TextChanged?.Invoke(this, EventArgs.Empty)
                    )
                );
            });

            textbox.TextChanged += (s, e) =>
            {
                if(_enabled)
                    _delayTimer.Change(DelayMilliSeconds, System.Threading.Timeout.Infinite);
            };
        }

        /// <summary>
        /// TextBoxTypeAssistant constructor
        /// </summary>
        /// <param name="textbox"></param>
        public TextBoxTypeAssistant(TextBox textbox)
            : this(textbox, 600)
        { }

        #endregion
    }
}
