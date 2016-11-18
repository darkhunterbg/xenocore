#region File Description
//-----------------------------------------------------------------------------
// GraphicsDeviceControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XenoCore.Builder.Host
{
    // System.Drawing and the XNA Framework both define Color and Rectangle
    // types. To avoid conflicts, we specify exactly which ones to use.
    using Color = System.Drawing.Color;
    using Rectangle = Microsoft.Xna.Framework.Rectangle;


    /// <summary>
    /// Custom control uses the XNA Framework GraphicsDevice to render onto
    /// a Windows Form. Derived classes can override the Initialize and Draw
    /// methods to add their own drawing code.
    /// </summary>
    public partial class GraphicsDeviceControl : UserControl 
    {
        private SwapChainRenderTarget renderTarget;

        #region Initialization

        public static GraphicsDevice GraphicsDevice { get; private set; }

        public static event EventHandler<GraphicsDevice> OnDeviceCreated;

        protected override void OnCreateControl()
        {
            // Don't initialize the graphics device if we are running in the designer.
            if (!DesignMode)
            {

                if (GraphicsDevice == null)
                {
                    GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, new PresentationParameters()
                    {
                        //BackBufferWidth = Math.Max(ClientSize.Width, 1),
                        //BackBufferHeight = Math.Max(ClientSize.Height, 1),
                        DeviceWindowHandle = Handle,
                    });
                    OnDeviceCreated?.Invoke(this, GraphicsDevice);
                    //   GraphicsService.Initialize(device);
                }

                renderTarget = new SwapChainRenderTarget(GraphicsDevice, this.Handle, ClientSize.Width, ClientSize.Height);

                Initialize();
            }



            base.OnCreateControl();
        }

        #endregion

        #region Paint



        protected override void Dispose(bool disposing)
        {
            renderTarget?.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnResize(EventArgs e)
        {
            if (renderTarget == null)
                return;
            renderTarget.Dispose();
            renderTarget = new SwapChainRenderTarget(GraphicsDevice, this.Handle, ClientSize.Width, ClientSize.Height);
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Enabled)
            {
                string beginDrawError = BeginDraw();

                if (string.IsNullOrEmpty(beginDrawError))
                {
                    // Draw the control using the GraphicsDevice.
                    Draw();
                    EndDraw();
                }
                else
                {
                    // If BeginDraw failed, show an error message using System.Drawing.
                    PaintUsingSystemDrawing(e.Graphics, beginDrawError);
                }
            }
            else
            {
                //PaintUsingSystemDrawing(e.Graphics, String.Empty);
            }
        }

        string BeginDraw()
        {
            // If we have no graphics device, we must be running in the designer.
            if (DesignMode)
            {
                return Text + "\n\n" + GetType();
            }

            // Make sure the graphics device is big enough, and is not lost.
            string deviceResetError = HandleDeviceReset();

            if (!string.IsNullOrEmpty(deviceResetError))
            {
                return deviceResetError;
            }

       

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.PresentationParameters.BackBufferWidth = ClientSize.Width;
            GraphicsDevice.PresentationParameters.BackBufferHeight = ClientSize.Height;
            GraphicsDevice.Viewport = new Viewport()
            {
                X = 0,
                Y = 0,
                Width = ClientSize.Width,
                Height = ClientSize.Height,
                MinDepth = 0,
                MaxDepth = 1
            };

            // GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            return null;
        }


        void EndDraw()
        {
            try
            {
                //  GraphicsDevice.SetRenderTarget(null);

                renderTarget.Present();
               // GraphicsDevice.Present();
            }
            catch
            {
            }
        }


        string HandleDeviceReset()
        {
            bool deviceNeedsReset = false;

            switch (GraphicsDevice.GraphicsDeviceStatus)
            {
                case GraphicsDeviceStatus.Lost:
                    // If the graphics device is lost, we cannot use it at all.
                    return "Graphics device lost";

                case GraphicsDeviceStatus.NotReset:
                    // If device is in the not-reset state, we should try to reset it.
                    deviceNeedsReset = true;
                    break;

                default:
                    return null;
                    // If the device state is ok, check whether it is big enough.
                    //PresentationParameters pp = GraphicsDevice.PresentationParameters;

                    //deviceNeedsReset = (ClientSize.Width > pp.BackBufferWidth) ||
                    //                   (ClientSize.Height > pp.BackBufferHeight);
                  //  break;
            }

            // Do we need to reset the device?
            if (deviceNeedsReset)
            {
                try
                {
                    GraphicsDevice.Reset(GraphicsDevice.PresentationParameters);
                   // renderTarget = new SwapChainRenderTarget(GraphicsDevice, this.Handle, ClientSize.Width, ClientSize.Height);
                }
                catch (Exception e)
                {
                    return "Graphics device reset failed\n\n" + e;
                }
            }

            return null;
        }


        /// <summary>
        /// If we do not have a valid graphics device (for instance if the device
        /// is lost, or if we are running inside the Form designer), we must use
        /// regular System.Drawing method to display a status message.
        /// </summary>
        protected virtual void PaintUsingSystemDrawing(Graphics graphics, string text)
        {
            graphics.Clear(Color.CornflowerBlue);

            using (Brush brush = new SolidBrush(Color.Black))
            {
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    graphics.DrawString(text, Font, brush, ClientRectangle, format);
                }
            }
        }


        /// <summary>
        /// Ignores WinForms paint-background messages. The default implementation
        /// would clear the control to the current background color, causing
        /// flickering when our OnPaint implementation then immediately draws some
        /// other color over the top using the XNA Framework GraphicsDevice.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }


        #endregion


        /// <summary>
        /// Derived classes override this to initialize their drawing code.
        /// </summary>
        protected virtual void Initialize() { }


        /// <summary>
        /// Derived classes override this to draw themselves using the GraphicsDevice.
        /// </summary>
        protected virtual void Draw()
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
        }

    }
}
