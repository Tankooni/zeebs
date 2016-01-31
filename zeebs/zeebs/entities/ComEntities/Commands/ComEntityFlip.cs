using Indigo.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.entities.ComEntities.Commands
{
    public class ComEntityFlip : ComEntityCommand
    {
        bool isDone = false;
        public ComEntityFlip(ComEntity comEntity)
            : base(comEntity)
        { }

        public override bool IsDone()
        {
            return isDone;
        }

        public override IEnumerator Update()
        {
            comEntity.IsFlipped = !comEntity.IsFlipped;
            isDone = true;
			yield return CoroutineHost.WaitForSeconds(.5f);
		}
    }
}
