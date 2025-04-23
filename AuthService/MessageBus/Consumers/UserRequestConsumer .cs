using JwtAuthenticationManager.Data;
using MassTransit;
using RabbitMQ.Contracts.Events.User;

public class UserRequestConsumer : IConsumer<GetUserRequestEvent>
{
    private readonly UserDbContext _dbContext;

    public UserRequestConsumer(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<GetUserRequestEvent> context)
    {
        var user = await _dbContext.Users.FindAsync(context.Message.UserId);
        var response = new UserInfoResponseEvent
        {
            UserId = context.Message.UserId,
            FullName = user?.FullName ?? "Unknown User"
        };

        await context.RespondAsync(response);
    }
}
