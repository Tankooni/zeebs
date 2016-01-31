using Indigo.Components;
using Indigo.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.entities.ComEntities.Commands
{
	class ComEntityChangeColor : ComEntityCommand
	{
		string color;
		bool isDone = false;
		public ComEntityChangeColor(ComEntity comEntity, string newColor)
            : base(comEntity)
        {
			color = newColor;
		}

		public override bool IsDone()
		{
			return isDone;
		}

		public override IEnumerator Update()
		{
			comEntity.SetColorTint(new Color(int.Parse(comEntity.TwitchUserComEntityData.TwitchUserColor = color, System.Globalization.NumberStyles.HexNumber)));
			isDone = true;
			yield return CoroutineHost.WaitForSeconds(.5f);
		}
	}
}
