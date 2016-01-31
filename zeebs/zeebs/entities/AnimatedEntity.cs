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
using Glide;
using Indigo.Rendering;

namespace zeebs.entities
{
	public class AnimatedEntity : Entity
	{
		public AnimatedEntityData AnimatedEntityData;
		public Dictionary<string, AnimationData> Animations = new Dictionary<string, AnimationData>();
		public Flipbook Sprite { get; set; }
		public Image Head;
		string currentHeadName;
		List<Image> images;
		Tween currentHeadTween;

        private float rotation;
        public float Rotation
        {
            get {return rotation;}
            set {
                while (rotation >= 360)
                {
                    rotation -= 360;
                }
                while (rotation < 0)
                {
                    rotation += 360;
                }

                rotation = value;
                this.SetRotation(rotation);
            }
        }

        private bool isFlipped;
        public bool IsFlipped
        {
            get { return isFlipped; }
            set
            {
                SetFlip(value);
                isFlipped = value;
            }
        }

		public AnimatedEntity(string entityName, string twitchHeadName)
			: this(entityName, twitchHeadName, Color.White) { }

		public AnimatedEntity(string entityName, string twitchHeadName, Color tintColor)
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
					Image newImage;
					images.Add(newImage = new Image(Library.GetTexture(animFilePath)) { OriginX = ad.Origin.X, OriginY = ad.Origin.Y });
					Shader chromaKey = new Shader(Shader.ShaderType.Fragment, Library.GetText("content/shaders/ChromaKey.frag"));
					newImage.Shader = chromaKey;
					//chromaKey.SetAsCurrentTexture("sampler2D");
					chromaKey.SetParameter("color", tintColor);

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

		public void SetAlpha(float value)
		{
			foreach(var image in images)
			{
				image.Alpha = value;
				Head.Alpha = value;
			}
		}

        public void SetRotation(float value)
        {
            foreach (var image in images)
            {
                image.Angle = rotation;
                Head.Angle = rotation;
            }
        }

        public void SetFlip(bool value)
        {
            foreach (var image in images)
            {
                image.FlippedX = value;
                Head.FlippedX = value;
                if (value != isFlipped)
                {
                    Head.X = -Head.X;
                }
            }
        }

		public void SetColorTint(Color tintColor)
		{
			foreach(var image in images)
				image.Shader.SetParameter("color", tintColor);
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
			if (currentHeadName != headName)
			{
				var oldHead = Head;
				if (!CreateHead(headName))
				{
					return false;
				}
				RemoveComponent(oldHead);
			}
			return true;
		}

		public bool CreateHead(string headName)
		{
			try
			{
				AddComponent(Head = new Image(Library.GetTexture("twitch//" + headName)));
				Head.CenterOrigin();
				Head.Scale = Animations[Sprite.CurrentAnim].HeadWidth / (float)Head.Width;
				
				Head.X = Animations[Sprite.CurrentAnim].HeadPosition.X;
				Head.Y = Animations[Sprite.CurrentAnim].HeadPosition.Y;

				//if (FP.Random.Bool())
				//	OnDownComplete();
				//else
				currentHeadTween = Tweener.Tween(Head, new { Y = Animations[Sprite.CurrentAnim].HeadPosition.Y - 4 }, FP.Random.Float(.4f, .7f));
				currentHeadTween.Ease(Ease.ToAndFro);
				currentHeadTween.Repeat();

				currentHeadName = headName;
				return true;
			}
			catch (Exception ex)
			{
#if DEBUG
				Console.WriteLine("Cannot Find: " + headName);
				Console.WriteLine(ex);
#endif
				//throw (ex);
				return false;
			}
		}

		public override void Update()
		{
			base.Update();

			
		}
	}
}
