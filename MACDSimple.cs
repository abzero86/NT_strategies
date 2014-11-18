#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// Enter the description of your strategy here
    /// </summary>
    [Description("Enter the description of your strategy here")]
    public class MACDSimple : Strategy
    {
        #region Variables
        // Wizard generated variables
        private int mACDFast = 12; // Default setting for MACDFast
        private int mACDSlow = 26; // Default setting for MACDSlow
        private int mACDSingle = 9; // Default setting for MACDSingle
        private int donchianPeriod = 20; // Default setting for DonchianPeriod
        // User defined variables (add any user defined variables below)
		private double StopLossPrice;
		private double TakeProfitPrice;
		private bool MACDCrossAbove = false;
		private bool MACDCrossBelow = false;
        #endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            //Add(DonchianChannel(DonchianPeriod));
            Add(MACD(MACDFast, MACDSlow, MACDSingle));
			Add(SMA(MACDFast));
            CalculateOnBarClose = true;
        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			// Signals
			if(CrossAbove(MACD(MACDFast, MACDSlow, MACDSingle).Diff, 0.005, 1))
			{
				MACDCrossAbove = true;
				MACDCrossBelow = false;
			}
			if(CrossBelow(MACD(MACDFast, MACDSlow, MACDSingle).Diff, -0.005, 1))
			{
				MACDCrossAbove = false;
				MACDCrossBelow = true;
			}
			if(MACD(MACDFast, MACDSlow, MACDSingle).Diff[1] > MACD(MACDFast, MACDSlow, MACDSingle).Diff[0])
			{
				MACDCrossAbove = false;
			}
			if(MACD(MACDFast, MACDSlow, MACDSingle).Diff[1] < MACD(MACDFast, MACDSlow, MACDSingle).Diff[0])
			{
				MACDCrossBelow = false;
			}
			
            // Enter Market
            if (MACDCrossAbove)
            {
                EnterLong(DefaultQuantity, "");
				StopLossPrice = DonchianChannel(DonchianPeriod).Lower[1];
				if (StopLossPrice > Close[0])
				{StopLossPrice = 0;}					
				TakeProfitPrice = (Close[0]-DonchianChannel(DonchianPeriod).Lower[1])*2+Close[0];
            }
            if (MACDCrossBelow)
            {
                EnterShort(DefaultQuantity, "");
				StopLossPrice = DonchianChannel(DonchianPeriod).Upper[1];
				if (StopLossPrice < Close[0])
				{StopLossPrice = 0;}
				TakeProfitPrice = Close[0]-(DonchianChannel(DonchianPeriod).Upper[1]-Close[0])*2;
            }

            // Exit
            if (Position.MarketPosition == MarketPosition.Long
				&& StopLossPrice > 0
                && Close[0] < StopLossPrice)
            {
                ExitLong("", "");
            }
            if (Position.MarketPosition == MarketPosition.Short
				&& StopLossPrice > 0
                && Close[0] > StopLossPrice)
            {
                ExitShort("", "");
            }
            if (Position.MarketPosition == MarketPosition.Long
				&& TakeProfitPrice > 0
                && Close[0] > TakeProfitPrice)
            {
                ExitLong("", "");
            }
            if (Position.MarketPosition == MarketPosition.Short
				&& TakeProfitPrice > 0
                && Close[0] < TakeProfitPrice)
            {
                ExitShort("", "");
            }
        }

        #region Properties
        [Description("")]
        [GridCategory("Parameters")]
        public int MACDFast
        {
            get { return mACDFast; }
            set { mACDFast = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDSlow
        {
            get { return mACDSlow; }
            set { mACDSlow = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int MACDSingle
        {
            get { return mACDSingle; }
            set { mACDSingle = Math.Max(1, value); }
        }

        [Description("")]
        [GridCategory("Parameters")]
        public int DonchianPeriod
        {
            get { return donchianPeriod; }
            set { donchianPeriod = Math.Max(1, value); }
        }
        #endregion
    }
}
