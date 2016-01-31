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
			this.moveTo = moveTo;
		}

		public override bool IsDone()
		{
			return FP.Distance(comEntity.X, comEntity.Y, comEntity.X, comEntity.Y) < 1;
		}

		public override void Update()
		{
			Indigo.Utils.Approach.TowardsWithDecay(ref comEntity.X, moveTo.X);
			Indigo.Utils.Approach.TowardsWithDecay(ref comEntity.Y, moveTo.Y);
		}
	}
}
