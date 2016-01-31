using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glide;
using Indigo;
using Indigo.Core;

namespace zeebs.entities
{
	public class ComEntityMoveTo : ComEntityCommand
	{
		Point moveTo;
		public ComEntityMoveTo(ComEntity comEntity, Point moveTo)
			: base(comEntity)
		{
			if (moveTo.X > FP.Width)
				moveTo.X = FP.Width;
			else if (moveTo.X < 0)
				moveTo.X = 0;
			if (moveTo.Y > FP.Height)
				moveTo.Y = FP.Height;
			else if (moveTo.Y < 0)
				moveTo.Y = 0;
			
			
			this.moveTo = moveTo;
		}

		public override bool IsDone()
		{
			return FP.Distance(comEntity.X, comEntity.Y, moveTo.X, moveTo.Y) < 1;
		}

		public override void Update()
		{
			Indigo.Utils.Approach.TowardsWithDecay(ref comEntity.X, moveTo.X);
			Indigo.Utils.Approach.TowardsWithDecay(ref comEntity.Y, moveTo.Y);
		}
	}
}
