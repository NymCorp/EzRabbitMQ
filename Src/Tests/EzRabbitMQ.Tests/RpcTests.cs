using System;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class RpcTests: TestBase
    {
        private readonly ITestOutputHelper _output;

        public RpcTests(ITestOutputHelper output) => _output = output;

        [Theory]
        [MemberData(nameof(RpcConfig))]
        public async Task CanDoRpcClientCall(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);

            using var server = mailbox.RpcServer<RpcServerTest>();

            using var client = mailbox.RpcClient();

            var message = Guid.NewGuid().ToString();

            var response = await client.CallAsync<RpcSampleResponse>(new RpcSampleRequest {Text = message}, default);

            Assert.Equal(message, response?.Model.Text);
        }

        [Theory]
        [MemberData(nameof(RpcConfig))]
        public void CanDoMultipleRpcClientCall(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);

            using var server = mailbox.RpcServer<RpcServerTest>();

            using var client = mailbox.RpcClient();

            var message = Guid.NewGuid().ToString();

            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message});

            Assert.NotNull(response);

            Assert.Equal(message, response.Model.Text);

            using var client2 = mailbox.RpcClient();

            var message2 = Guid.NewGuid().ToString();

            var response2 = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message2});

            Assert.Equal(message2, response2?.Model.Text);
        }

        [Theory]
        [MemberData(nameof(RpcConfig))]
        public void CanDoMultipleRpcClientCalls(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);

            using var server = mailbox.RpcServer<RpcServerTest>();

            using var client = mailbox.RpcClient();

            var message = Guid.NewGuid().ToString();

            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message});

            Assert.NotNull(response);

            Assert.Equal(message, response.Model.Text);

            var message2 = Guid.NewGuid().ToString();

            var response2 = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message2});

            Assert.NotNull(response2);

            Assert.Equal(message2, response2.Model.Text);
        }

        [Theory]
        [MemberData(nameof(RpcConfig))]
        public void CanDoTimeout(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);

            using var client = mailbox.RpcClient();

            var message = Guid.NewGuid().ToString();

            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message});

            Assert.Null(response);
        }

        [Theory]
        [MemberData(nameof(RpcConfig))]
        public void CanUseMultipleRpcServer(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);

            using var serverA = mailbox.RpcServer<RpcServerTest>();
            using var serverB = mailbox.RpcServer<RpcServerTest>("serverB");

            using var client = mailbox.RpcClient();
            using var clientB = mailbox.RpcClient("serverB");

            var messageA = Guid.NewGuid().ToString();
            var messageB = Guid.NewGuid().ToString();

            var responseB = clientB.Call<RpcSampleResponse>(new RpcSampleRequest {Text = messageB});
            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = messageA});

            Assert.Equal(messageA, response?.Model.Text);
            Assert.Equal(messageB, responseB?.Model.Text);
        }
        
        [Theory]
        [MemberData(nameof(RpcConfig))]
        public void CanUseMultipleRpcClient(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);

            using var serverA = mailbox.RpcServer<RpcServerTest>();

            using var client = mailbox.RpcClient();
            using var clientB = mailbox.RpcClient();

            var messageA = Guid.NewGuid().ToString();
            var messageB = Guid.NewGuid().ToString();

            var responseB = clientB.Call<RpcSampleResponse>(new RpcSampleRequest {Text = messageB});
            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = messageA});

            Assert.Equal(messageA, response?.Model.Text);
            Assert.Equal(messageB, responseB?.Model.Text);
        }

        [Theory]
        [MemberData(nameof(RpcConfig))]
        public void CanReceiveMultipleHandleInOneServer(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);

            using var client = mailbox.RpcClient();
            using var serverA = mailbox.RpcServer<RpcServerTest>();

            var message = Guid.NewGuid().ToString();
            var response = client.Call<RpcIncrementResponse>(new RpcIncrementRequest(1));
            var response2 = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message});

            Assert.Equal(2, response?.NewValue);
            Assert.Equal(message, response2?.Model.Text);
        }


        [Theory]
        [MemberData(nameof(RpcConfig))]
        public void CanSendMultipleMessage(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);

            using var client = mailbox.RpcClient();
            using var serverA = mailbox.RpcServer<RpcServerTest>();

            for (var i = 0; i < 100; i++)
            {
                var response = client.Call<RpcIncrementResponse>(new RpcIncrementRequest(i));
                Assert.Equal(i + 1, response?.NewValue);
            }
        }

        [Theory]
        [MemberData(nameof(RpcConfig))]
        public void CanSendMessageThatIsNotExcepted(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);
            
            using var client = mailbox.RpcClient();
            using var serverA = mailbox.RpcServer<RpcServerTest>();
            
            var response = client.Call<RpcUnexpectedResponse>(new RpcUnexpectedRequest());
            
            Assert.Null(response);
        }
        
        [Theory]
        [MemberData(nameof(RpcConfig))]
        public void CanRecoverFromUnexpectedMessage(bool isRetryHandle, bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, isRetryHandle, isAsync);
            
            using var serverA = mailbox.RpcServer<RpcServerTest>();
            using var client = mailbox.RpcClient();
            
            var unexpectedResponse = client.Call<RpcUnexpectedResponse>(new RpcUnexpectedRequest());
            var response = client.Call<RpcIncrementResponse>(new RpcIncrementRequest(0));

            Assert.Equal(1, response?.NewValue);
            Assert.Null(unexpectedResponse);
        }
    }
}