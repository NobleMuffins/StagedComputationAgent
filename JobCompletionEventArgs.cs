using System;

namespace NobleMuffins.StagedComputationKit {
	public class JobCompletionEventArgs : EventArgs {

		public readonly Job job;

		public JobCompletionEventArgs(Job job): base()
		{
			this.job = job;
		}
	}
}