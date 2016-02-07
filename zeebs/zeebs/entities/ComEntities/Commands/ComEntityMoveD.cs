using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glide;
using Indigo;
using Indigo.Core;
using System.Collections;

namespace zeebs.entities.ComEntities.Commands
{
    public class ComEntityMoveD : ComEntityMoveTo
    {
        public ComEntityMoveD(ComEntity comEntity, Point moveDelta, string moverUserName = null)
            : base(comEntity, moveDelta, moverUserName)
        {
        }

		public override void Init()
		{
			moveTo = new Point(moveTo.X + comEntity.X, moveTo.Y + comEntity.Y);
		}
    }
}
