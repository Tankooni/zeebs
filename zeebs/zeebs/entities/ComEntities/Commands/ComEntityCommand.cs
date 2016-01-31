using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zeebs.entities
{
	public abstract class ComEntityCommand
	{
		public ComEntity comEntity;

		public ComEntityCommand(ComEntity comEntity)
		{
			this.comEntity = comEntity;
		}

		public virtual bool IsDone()
		{
			return true;
		}

		/// <summary>
		/// This is for the purpose of allowing for external things to mark this job as done
		/// </summary>
		public virtual void CallWhenDone()
		{

		}

		public virtual IEnumerator Update()
		{
			yield return null;
		}

		public virtual void Interrupt()
		{

		}
	}
}
