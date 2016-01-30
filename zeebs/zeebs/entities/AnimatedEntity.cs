using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo;
using Utils.Json;
using zeebs.metaData;
using Tankooni;
using Indigo.Graphics;

namespace zeebs.entities
{
	public class AnimatedEntity : Entity
	{
		public AnimatedEntityData AnimatedEntityData;
		public Dictionary<string, AnimationData> Animations = new Dictionary<string, AnimationData>();
		public Flipbook Sprite { get; set; }
		public Image Head;
		List<Image> images;

		public AnimatedEntity(string entityName, string twitchHeadName)
		{
			images = new List<Image>();
			string EntityFilePath = Utility.CONTENT_DIR + "/entities/" + entityName;
			AnimatedEntityData = JsonLoader.Load<AnimatedEntityData>(EntityFilePath + "/MetaData");
			foreach(var animation in AnimatedEntityData.Animations)
			{
				string animationPath = EntityFilePath + "/" + AnimatedEntityData.DefaultAnimation;
				AnimationData ad = JsonLoader.Load<AnimationData>(animationPath + "/MetaData");
				Animations.Add(ad.Name, ad);
				foreach(var animFilePath in Utility.RetrieveFilePathForFilesInDirectory(@"./" + animationPath, "*.png"))
				{
					//int currentTotalFrames = 0;
					//foreach (var animation in MetaData.Animations)
					//	sprite.Add(animation.Name, FP.MakeFrames(currentTotalFrames, (currentTotalFrames += animation.Frames) - 1), animation.FPS, true);
					//AddComponent(sprite);
					images.Add(new Image(Library.GetTexture(animFilePath)) { OriginX = ad.Origin.X, OriginY = ad.Origin.Y });
					//Console.WriteLine(animFile);
				}
			}

			Sprite = new Flipbook(images.Cast<Graphic>().ToArray());

			int currentTotalFrames = 0;
			foreach (var animation in AnimatedEntityData.Animations)
				Sprite.Add(animation, FP.MakeFrames(currentTotalFrames, (currentTotalFrames += Animations[animation].Frames) - 1), Animations[animation].FPS, true);
			Sprite.Play(AnimatedEntityData.DefaultAnimation);
			AddComponent(Sprite);

			CreateHead(twitchHeadName);
		}

		public void PlayAnmation(string animation)
		{
			if (animation == Sprite.CurrentAnim)
				return;
			Sprite.Play(animation);

			if (Head != null)
			{
				Head.X = Animations[animation].HeadPosition.X;
				Head.Y = Animations[animation].HeadPosition.Y;
			}
		}

		public bool ChangeHead(string headName)
		{
			if (Head == null)
			{
				if (!CreateHead(headName))
				{
					return false;
				}
			}
			return true;

		}

		public bool CreateHead(string headName)
		{
			try
			{
				AddComponent(Head = new Image(Library.GetTexture("twitch//" + headName)) { X = Animations[Sprite.CurrentAnim].HeadPosition.X, Y = Animations[Sprite.CurrentAnim].HeadPosition.Y });
				Head.CenterOrigin();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Cannot Find: " + headName);
				Console.WriteLine(ex);
				//throw (ex);
				return false;
			}
		}
	}
}
