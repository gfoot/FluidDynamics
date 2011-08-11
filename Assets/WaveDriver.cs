public class WaveDriver : ISimulationDriver
{
    private IFluidSimulation _simulation;

    private bool _fixColumn0Height;

    public WaveDriver (IFluidSimulation simulation)
    {
        _simulation = simulation;

        for (int y = 0; y < _simulation.Height; ++y)
        {
            for (int x = 0; x < _simulation.Width; ++x)
            {
                _simulation.AddWater(x, y, 2.0f);
            }
        }

        _fixColumn0Height = true;
    }

    public void Update()
    {
        if (!_fixColumn0Height)
            return;

        float lambda = (UnityEngine.Time.realtimeSinceStartup - 1) / 0.2f;
        float height;
        if (lambda < 0)
        {
            height = 0;
        }
        else if (lambda < 1)
        {
            height = lambda;
        }
        else if (lambda < 4)
        {
            height = 1;
        }
        else if (lambda < 5)
        {
            height = 5 - lambda;
        }
        else
        {
            height = 0;
            _fixColumn0Height = false;
        }

        _simulation.AddWater(0, 0, height * 20);
    }
}
