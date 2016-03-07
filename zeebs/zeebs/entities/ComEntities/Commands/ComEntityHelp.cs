using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.entities.ComEntities.Commands
{
    public class ComEntityHelp : ComEntityCommand
    {
        bool isDone = false;
        public ComEntityHelp(ComEntity comEntity)
            : base(comEntity)
        { }

        public override bool IsDone()
        {
            return isDone;
        }

        public override IEnumerator Update()
        {
            yield return null;
        }
    }
}
