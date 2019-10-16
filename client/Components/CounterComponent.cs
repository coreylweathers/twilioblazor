using Microsoft.AspNetCore.Components;

namespace client.Components
{
    public class CounterComponent : ComponentBase
    {
        public int currentCount = 0;

        public void IncrementCount()
        {
            currentCount++;
        }
    }
}