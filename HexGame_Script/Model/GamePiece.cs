using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
	// include animal / human
    public class GamePiece : SpacialObject
    {
		
		public int m_StrongValue{ get; set; }
		
		// Blood 
        public GamePiece(Point location)
            : base(location)
        {
			m_StrongValue=1;
        }
    }

}