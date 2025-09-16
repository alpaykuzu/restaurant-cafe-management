using Microsoft.AspNetCore.SignalR;

namespace Core.Hubs
{
    public class RestaurantHub : Hub
    {
        public async Task JoinRestaurantGroup(int restaurantId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, restaurantId.ToString());
        }
    }
}
