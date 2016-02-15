using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.entities.ComEntities.Commands
{
	class ComEntityAttack : ComEntityCommand
	{
		bool isDone = false;
		public ComEntityAttack(ComEntity comEntity)
            : base(comEntity)
        {
		}

		public override bool IsDone()
		{
			return isDone;
		}

		public override IEnumerator Update()
		{
			float deltaSpin = comEntity.Rotation;
			Indigo.Utils.Approach.TowardsWithDecay(ref deltaSpin, 360, .3f);
			comEntity.Rotation = deltaSpin;
			if (deltaSpin >= 359)
			{
				comEntity.Rotation = 0;
				isDone = true;
			}
			yield return null;
		}
	}
}
