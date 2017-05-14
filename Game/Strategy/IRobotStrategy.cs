using Game.State;

namespace Game.Strategy
{
	public interface IRobotStrategy
	{
		IRobotStrategy Process(TurnState turnState);
		void Dump(string gameStateRef);
	}
}