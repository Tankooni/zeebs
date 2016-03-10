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
		bool isAvatar = false;
		public ComEntityChangeHead(ComEntity comEntity, string headToChangeTo, bool isAvatar)
            : base(comEntity)
        {
			this.headToChangeTo = headToChangeTo;
			this.isAvatar = isAvatar;
		}

		public override bool IsDone()
		{
			return isDone;
		}

		public override IEnumerator Update()
		{
			comEntity.ChangeHead(headToChangeTo, isAvatar);
			isDone = true;
			yield return CoroutineHost.WaitForSeconds(.2f);
		}
	}
}
