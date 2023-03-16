﻿namespace SotnRandoTools.Services
{
	public interface IInputService
	{
		bool RegisteredMove(string moveName, int frames);
		bool ButtonPressed(string button, int frames);
		bool ButtonReleased(string button, int frames);
		bool ButtonHeld(string button);
		void UpdateInputs();
	}
}