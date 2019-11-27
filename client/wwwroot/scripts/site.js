window.appMethods = {
    chat: {
        dotNetReference: null,
        client: null,
        saveToken: function (key, value) {
            let storage = window.sessionStorage;
            storage.setItem(key, value);
            console.log('Saved item to storage');
        },
        getToken: function (tokenName) {
            let value = window.sessionStorage.getItem(tokenName);
            return value;
        },
        init: function (token, dotNet) {
            this.saveDotNetInstance(dotNet);
            Twilio.Chat.Client.create(token)
                .then(client => {
                    console.log('Hey we have a client', client);
                    // Put the client in some thing some where
                    this.client = client;
                    this.getChannels();
                    // Create methods to do the send message function
                })
                .catch(err => {
                    console.log('An error has occurred', err);
                });
        },
        saveDotNetInstance: function (dotnet) {
            this.dotNetReference = dotnet;
        },
        getChannels: function () {
            this.client.getSubscribedChannels().then(results => {
                console.log('Loaded the channels', results);
                let items = [];
                results.items.forEach(item => {
                    console.log('Channel Item', item);
                    items.push({ 'sid': item.sid, 'friendlyName': item.friendlyName, 'uniqueName': item.uniqueName });
                });
                const data = JSON.stringify(items);
                console.log('Calling into .NET Method with items', data);
                this.dotNetReference.invokeMethodAsync('SaveChatChannels', data);
            })
                .catch(err => {
                    console.error(err);
                });
        },
        joinChannel: function () {

        },
        createOrJoinChannel: function (channelName, friendlyName = 'Default Friendly Name') {
            this.client.getChannelByUniqueName(channelName)
                .then(channel => {
                    // Channel would already exist
                    this.setupChannel(channel);
                })
                .catch(err => {
                    // Channel does not exist. Create if necessary
                    console.log('Channel does not exist yet');
                    this.client.createChannel({
                        uniqueName: channelName,
                        friendlyName: friendlyName
                    }).then(channel => {
                        console.log('The channel was created', channel);
                        this.setupChannel(channel);
                    });
                });
        },
        setupChannel: function (channel) {
            channel.join().then(() => {
                console.log('We successfully joined the channel');
                this.dotNetReference.invokeMethodAsync('Chat', 'SaveChatChannels', channels);
            });

            channel.on('messageAdded', () => {
                console.log('Sending the message back into the c# code to update the list of messages');
            });
        }
    },
    call: {
        setup: function (token) {
            console.log('Getting connected');

            // Setup Twilio Device
            Twilio.Device.setup(token);

            Twilio.Device.ready(() => {
                console.log('We are connected and ready to do the thing');
            });

            Twilio.Device.error((err) => {
                console.error('This should not have been reached. We need to do something here');
                console.error(err);
            });
        },
        placeCall: function (destination) {
            console.log(`Calling ${destination}`);
            Twilio.Device.connect({ phone: destination });
            console.log(`Successfully called ${destination}`);
        },
        endCall: function () {
            console.log('Ending the call');
            Twilio.Device.disconnectAll();
            console.log('Successfully ended the call');
        }
    }
};
