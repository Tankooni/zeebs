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
    public class ComEntityMoveD : ComEntityCommand
    {
        Point movePoint;
        Point targetPosition;
        bool isTargeted = false;
        public ComEntityMoveD(ComEntity comEntity, Point movePoint)
            : base(comEntity)
        {
            this.movePoint = movePoint;
        }

        public override bool IsDone()
        {
            if(!isTargeted) return false;
            return FP.Distance(comEntity.X, comEntity.Y, targetPosition.X, targetPosition.Y) < 1;
        }

        public override IEnumerator Update()
        {
            if (!isTargeted)
            {
                targetPosition = new Point(movePoint.X + comEntity.X, movePoint.Y + comEntity.Y);

                
                if (targetPosition.X > FP.Width)
                    targetPosition.X = FP.Width;
                else if (targetPosition.X < 0)
                    targetPosition.X = 0;
                if (targetPosition.Y > FP.Height)
                    targetPosition.Y = FP.Height;
                else if (targetPosition.Y < 0)
                    targetPosition.Y = 0;

                isTargeted = true;
            }

            Indigo.Utils.Approach.TowardsWithDecay(ref comEntity.X, targetPosition.X, 0.2f);
            Indigo.Utils.Approach.TowardsWithDecay(ref comEntity.Y, targetPosition.Y, 0.2f);
            yield return null;
        }
    }
}
