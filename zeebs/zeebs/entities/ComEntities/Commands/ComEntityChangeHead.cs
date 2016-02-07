using Indigo.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankooni;

namespace zeebs.entities.ComEntities.Commands
{
	public class ComEntityChangeHead : ComEntityCommand
	{
		bool isDone = false;
		string headToChangeTo;
		public ComEntityChangeHead(ComEntity comEntity, string headToChangeTo)
            : base(comEntity)
        {
			this.headToChangeTo = headToChangeTo;
		}

		public override bool IsDone()
		{
			return isDone;
		}

		public override IEnumerator Update()
		{
			comEntity.ChangeHead(headToChangeTo);
			isDone = true;
			yield return CoroutineHost.WaitForSeconds(.2f);
		}
	}
}
