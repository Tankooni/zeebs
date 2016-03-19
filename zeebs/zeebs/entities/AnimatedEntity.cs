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
		//public Flipbook Sprite { get; set; }
		public Spritemap Sprite { get; set; }
		public Image Head;
		string currentHeadName;
		Tween currentHeadTween;
        
        //Name that will appear above the zeeb
        public Text zeebName;

        private float rotation;
        public float Rotation
        {
            get { return rotation; }
            set
            {
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

		public AnimatedEntity(string entityName, string twitchHeadName, bool isAvatar, string twitchUsername)
			: this(entityName, twitchHeadName, isAvatar, twitchUsername, Color.White)
        {

        }

		public AnimatedEntity(string entityName, string twitchHeadName, bool isAvatar, string twitchUseranme, Color tintColor)
		{
			string EntityFilePath = Utility.CONTENT_DIR + "/entities/" + entityName;
			AnimatedEntityData = JsonLoader.Load<AnimatedEntityData>(EntityFilePath);
			var defaultAnim = AnimatedEntityData.Animations[AnimatedEntityData.DefaultAnimation];
			Sprite = new Spritemap(Library.GetTexture(EntityFilePath + ".png"), defaultAnim.FrameWidth, defaultAnim.FrameHeight)
			{
				OriginX = defaultAnim.OriginX,
				OriginY = defaultAnim.OriginY
			};

            foreach (var animation in AnimatedEntityData.Animations.Values)
				Sprite.Add(animation.Name, animation.Frames, animation.FPS, true);

			if (AnimatedEntityData.ShaderName != null)
			{
				Shader shader = new Shader(Shader.ShaderType.Fragment, Library.GetText("content/shaders/" + AnimatedEntityData.ShaderName));
				Sprite.Shader = shader;
				//chromaKey.SetAsCurrentTexture("sampler2D");
				shader.SetParameter("color", tintColor);
			}

			Sprite.Play(AnimatedEntityData.DefaultAnimation);
			AddComponent(Sprite);

			CreateHead(twitchHeadName, isAvatar);
            CreateName(twitchUseranme);
		}
        public void SetAlpha(float value)
		{
			Sprite.Alpha = value;
			if (Head != null)
				Head.Alpha = value;
		}

        public void SetRotation(float value)
        {
            Sprite.Angle = rotation;
			if(Head != null)
				Head.Angle = rotation;
        }

        public void SetFlip(bool value)
        {
                Sprite.FlippedX = value;
				if (Head != null)
				{
					Head.FlippedX = value;
					if (value != isFlipped)
					{
						Head.X = -Head.X;
					}
				}
        }

		public void SetColorTint(Color tintColor)
		{
			if(Sprite.Shader != null)
				Sprite.Shader.SetParameter("color", tintColor);
		}

		public void PlayAnmation(string animation)
		{
			if (animation == Sprite.CurrentAnim)
				return;

			Sprite.OriginX = AnimatedEntityData.Animations[animation].OriginX;
			Sprite.OriginY = AnimatedEntityData.Animations[animation].OriginY;

			Sprite.Play(animation);

			if (Head != null)
			{
				Head.X = AnimatedEntityData.Animations[animation].HeadPositionX;
				Head.Y = AnimatedEntityData.Animations[animation].HeadPositionY;
			}
		}

		public bool ChangeHead(string headName, bool isAvatar)
		{
			if (currentHeadName != headName)
			{
				var oldHead = Head;
				if (!CreateHead(headName, isAvatar))
				{
					return false;
				}
				if(oldHead != null)
					RemoveComponent(oldHead);
			}
			return true;
		}

		public bool CreateHead(string headName, bool isAvatar)
		{
			try
			{
				AddComponent(Head = new Image(Library.GetTexture((isAvatar ? "twitchAvatar//" : "twitch//") + headName)));
				Head.CenterOrigin();
				Head.Scale = AnimatedEntityData.Animations[Sprite.CurrentAnim].HeadWidth / (float)Head.Width;
				
				Head.X = AnimatedEntityData.Animations[Sprite.CurrentAnim].HeadPositionX;
				Head.Y = AnimatedEntityData.Animations[Sprite.CurrentAnim].HeadPositionY;

				currentHeadTween = Tweener.Tween(Head, new { Y = AnimatedEntityData.Animations[Sprite.CurrentAnim].HeadPositionY - 4 }, .7f);
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

        private void CreateName(string twitchUseranme)
        {
            AddComponent(zeebName = new Text(twitchUseranme) { Scale = 0.7f });

            zeebName.CenterOrigin();
            //zeebName.Scale = AnimatedEntityData.Animations[Sprite.CurrentAnim].HeadWidth / (float)zeebName.Width;

            zeebName.X = AnimatedEntityData.Animations[Sprite.CurrentAnim].HeadPositionX;
            zeebName.Y = AnimatedEntityData.Animations[Sprite.CurrentAnim].HeadPositionY - 17.0f;
        }


        public override void Update()
		{
			base.Update();

			
		}
	}
}
