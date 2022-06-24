using DSharpPlus.SlashCommands;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace LathBotFront.Interactions.PreExecutionChecks
{
    public class CooldownSlash
    {
        public int MaxUses { get; }
        public TimeSpan Reset { get; }
        public ConcurrentDictionary<string, CoolDownBucket> Buckets { get; }

        public CooldownSlash(double resetAfter, int maxUses = 1)
        {
            MaxUses = maxUses;
            Reset = TimeSpan.FromSeconds(resetAfter);
            Buckets = new ConcurrentDictionary<string, CoolDownBucket>();
        }

        public CoolDownBucket GetBucket(InteractionContext ctx)
        {
            var bucketId = GetBucketId(ctx, out _);
            Buckets.TryGetValue(bucketId, out var bucket);
            return bucket;
        }

        public TimeSpan GetRemainingCooldown(InteractionContext ctx)
        {
            var bucket = GetBucket(ctx);
            if (bucket is null)
                return TimeSpan.Zero;
            return bucket.ResetsAt - DateTimeOffset.UtcNow;
        }

        public string GetBucketId(InteractionContext ctx, out ulong userId)
        {
            userId = ctx.User.Id;
            return CoolDownBucket.MakeId(userId);
        }

        public async Task<bool> ExecuteCheckAsync(InteractionContext ctx)
        {
            var bucketId = GetBucketId(ctx, out var user);
            if (!Buckets.TryGetValue(bucketId, out var bucket))
            {
                bucket = new CoolDownBucket(MaxUses, Reset, user);
                Buckets.AddOrUpdate(bucketId, bucket, (k, v) => bucket);
            }

            return await bucket.DecrementUseAsync();
        }
    }

    public class CoolDownBucket
    {
        public ulong UserId { get; }
        public string BucketId { get; }
        public int RemainingUses
            => Volatile.Read(ref _remainingUses);
        public int MaxUses { get; }
        public DateTimeOffset ResetsAt { get; set; }
        public TimeSpan Reset { get; set; }
        public SemaphoreSlim UsageSemaphore { get; set; }

        private int _remainingUses;

        public CoolDownBucket(int maxUses, TimeSpan resetAfter, ulong userId = 0)
        {
            _remainingUses = maxUses;
            MaxUses = maxUses;
            ResetsAt = DateTimeOffset.UtcNow + resetAfter;
            Reset = resetAfter;
            UserId = userId;
            UsageSemaphore = new SemaphoreSlim(1, 1);
        }

        internal async Task<bool> DecrementUseAsync()
        {
            await UsageSemaphore.WaitAsync();

            var now = DateTimeOffset.UtcNow;
            if (now >= ResetsAt)
            {
                Interlocked.Exchange(ref _remainingUses, MaxUses);
                ResetsAt = now + Reset;
            }

            var success = false;
            if (RemainingUses > 0)
            {
                Interlocked.Decrement(ref _remainingUses);
                success = true;
            }

            UsageSemaphore.Release();
            return success;
        }

        public static string MakeId(ulong userId = 0)
            => $"{userId.ToString(CultureInfo.InvariantCulture)}";
    }
}
