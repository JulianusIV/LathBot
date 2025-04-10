using DSharpPlus.Commands.Processors.SlashCommands;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace LathBotFront.Commands.PreExecutionChecks
{
    public class CooldownSlash(double resetAfter, int maxUses = 1)
    {
        public int MaxUses { get; } = maxUses;
        public TimeSpan Reset { get; } = TimeSpan.FromSeconds(resetAfter);
        public ConcurrentDictionary<string, CoolDownBucket> Buckets { get; } = new ConcurrentDictionary<string, CoolDownBucket>();

        public CoolDownBucket GetBucket(SlashCommandContext ctx)
        {
            var bucketId = this.GetBucketId(ctx, out _);
            this.Buckets.TryGetValue(bucketId, out var bucket);
            return bucket;
        }

        public TimeSpan GetRemainingCooldown(SlashCommandContext ctx)
        {
            var bucket = this.GetBucket(ctx);
            if (bucket is null)
                return TimeSpan.Zero;
            return bucket.ResetsAt - DateTimeOffset.UtcNow;
        }

        public string GetBucketId(SlashCommandContext ctx, out ulong userId)
        {
            userId = ctx.User.Id;
            return CoolDownBucket.MakeId(userId);
        }

        public async Task<bool> ExecuteCheckAsync(SlashCommandContext ctx)
        {
            var bucketId = this.GetBucketId(ctx, out var user);
            if (!this.Buckets.TryGetValue(bucketId, out var bucket))
            {
                bucket = new CoolDownBucket(this.MaxUses, this.Reset, user);
                this.Buckets.AddOrUpdate(bucketId, bucket, (k, v) => bucket);
            }

            return await bucket.DecrementUseAsync();
        }
    }

    public class CoolDownBucket(int maxUses, TimeSpan resetAfter, ulong userId = 0)
    {
        public ulong UserId { get; } = userId;
        public string BucketId { get; }
        public int RemainingUses
            => Volatile.Read(ref this._remainingUses);
        public int MaxUses { get; } = maxUses;
        public DateTimeOffset ResetsAt { get; set; } = DateTimeOffset.UtcNow + resetAfter;
        public TimeSpan Reset { get; set; } = resetAfter;
        public SemaphoreSlim UsageSemaphore { get; set; } = new SemaphoreSlim(1, 1);

        private int _remainingUses = maxUses;

        internal async Task<bool> DecrementUseAsync()
        {
            await this.UsageSemaphore.WaitAsync();

            var now = DateTimeOffset.UtcNow;
            if (now >= this.ResetsAt)
            {
                Interlocked.Exchange(ref this._remainingUses, this.MaxUses);
                this.ResetsAt = now + this.Reset;
            }

            var success = false;
            if (this.RemainingUses > 0)
            {
                Interlocked.Decrement(ref this._remainingUses);
                success = true;
            }

            this.UsageSemaphore.Release();
            return success;
        }

        public static string MakeId(ulong userId = 0)
            => $"{userId.ToString(CultureInfo.InvariantCulture)}";
    }
}
