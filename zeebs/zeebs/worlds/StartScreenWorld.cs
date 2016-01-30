using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo;
using Indigo.Inputs;
using Indigo.Graphics;

namespace zeebs
{
	class StartScreenWorld : World
	{
		private Text start;
		private Text instructions;

		public StartScreenWorld()
		{
			start = new Text("Start [Enter]");
			start.X = (FP.Width / 2) - (start.Width / 2);
			start.Y = (FP.Height / 3) + 25;

			instructions = new Text("Instructions [Space]");
			instructions.X = (FP.Width / 2) - (instructions.Width / 2);
			instructions.Y = (FP.Height / 3) + 50;
			AddGraphic(start);
		}

		public override void Update()
		{
			base.Update();
			//if (Keyboard.Return.Pressed)
			//	FP.World = new DynamicSceneWorld(Utility.MainConfig.StartingScene, Utility.MainConfig.SpawnEntrance);

			//            if (Keyboard.Space.Pressed)
			//                FP.World = new InstructionsScreenWorld();
		}
	}
}
