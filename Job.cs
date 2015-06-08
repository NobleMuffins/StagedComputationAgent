using System;
using System.Collections;

namespace NobleMuffins.StagedComputationKit {
	public class Job
	{
		public readonly System.Action<object> function;
		public readonly IEnumerable elements;

		public Job(Action<object> function, IEnumerable elements)
		{
			this.function = function;
			this.elements = elements;
		}
	}
}