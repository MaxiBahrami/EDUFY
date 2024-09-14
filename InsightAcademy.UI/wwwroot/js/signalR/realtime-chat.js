var chatconnection;
var otherUserId;
var GroupName;
var messageCount = 0;
var CurrentUserId = "";
let index = 0;
function startHubConnection(recepientId) {
    showLoader();
    otherUserId = recepientId;

    chatconnection = new signalR.HubConnectionBuilder().withUrl(`/chat?user=${otherUserId}`).configureLogging(signalR.LogLevel.Information).build();
    chatconnection.start().catch(function (err) {
        showToast('error', err.toString())
    });
    chatconnection.on("ReceiveMessageThread", function (currentUserId, messages, groupName, recepientName, recepientImageUrl) {
        $('#loader').css('display', 'block')
        $('#tynQuickReply').empty();
        GroupName = "";
        CurrentUserId = currentUserId;
        GroupName = groupName;

        $('#recepient-name').text(recepientName);
        $('#recepient-image').attr('src', "/" + recepientImageUrl);
        messages.forEach(function (message) {
            const isCurrentUser = (currentUserId === message.senderId);
            let html;
            if (isCurrentUser) {
                if (message.type == 1) {
                    html = `<div class="tyn-reply-item outgoing">
                <div class="tyn-reply-group">
                    <div class="tyn-reply-bubble">

                        <div class="tyn-reply-text"> ${message.message} </div>
                    </div>
                </div>
            </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)} </div>
            `
                }
                else if (message.type == 2) //file
                {
                    html = ` <div class="tyn-reply-bubble outgoing">
                                    <div class="tyn-reply-file">
                                        <a href="#" onclick="downloadBase64File('${message.file}', '${message.fileName}')"  class="tyn-file">
                                            <div class="tyn-media-group">
                                                <div class="tyn-media tyn-size-lg text-bg-light">
                                                    <!-- filetype-docx -->
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-filetype-docx" viewBox="0 0 16 16">
                                                        <path fill-rule="evenodd" d="M14 4.5V11h-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5zm-6.839 9.688v-.522a1.5 1.5 0 0 0-.117-.641.86.86 0 0 0-.322-.387.86.86 0 0 0-.469-.129.87.87 0 0 0-.471.13.87.87 0 0 0-.32.386 1.5 1.5 0 0 0-.117.641v.522q0 .384.117.641a.87.87 0 0 0 .32.387.9.9 0 0 0 .471.126.9.9 0 0 0 .469-.126.86.86 0 0 0 .322-.386 1.55 1.55 0 0 0 .117-.642m.803-.516v.513q0 .563-.205.973a1.47 1.47 0 0 1-.589.627q-.381.216-.917.216a1.86 1.86 0 0 1-.92-.216 1.46 1.46 0 0 1-.589-.627 2.15 2.15 0 0 1-.205-.973v-.513q0-.569.205-.975.205-.411.59-.627.386-.22.92-.22.535 0 .916.22.383.219.59.63.204.406.204.972M1 15.925v-3.999h1.459q.609 0 1.005.235.396.233.589.68.196.445.196 1.074 0 .634-.196 1.084-.197.451-.595.689-.396.237-.999.237zm1.354-3.354H1.79v2.707h.563q.277 0 .483-.082a.8.8 0 0 0 .334-.252q.132-.17.196-.422a2.3 2.3 0 0 0 .068-.592q0-.45-.118-.753a.9.9 0 0 0-.354-.454q-.237-.152-.61-.152Zm6.756 1.116q0-.373.103-.633a.87.87 0 0 1 .301-.398.8.8 0 0 1 .475-.138q.225 0 .398.097a.7.7 0 0 1 .273.26.85.85 0 0 1 .12.381h.765v-.073a1.33 1.33 0 0 0-.466-.964 1.4 1.4 0 0 0-.49-.272 1.8 1.8 0 0 0-.606-.097q-.534 0-.911.223-.375.222-.571.633-.197.41-.197.978v.498q0 .568.194.976.195.406.571.627.375.216.914.216.44 0 .785-.164t.551-.454a1.27 1.27 0 0 0 .226-.674v-.076h-.765a.8.8 0 0 1-.117.364.7.7 0 0 1-.273.248.9.9 0 0 1-.401.088.85.85 0 0 1-.478-.131.83.83 0 0 1-.298-.393 1.7 1.7 0 0 1-.103-.627zm5.092-1.76h.894l-1.275 2.006 1.254 1.992h-.908l-.85-1.415h-.035l-.852 1.415h-.862l1.24-2.015-1.228-1.984h.932l.832 1.439h.035z" />
                                                    </svg>
                                                </div>
                                                <div class="tyn-media-col">
                                                    <h6 class="name">${message.fileName}</h6>
                                                    <div class="meta">${message.size} MB</div>
                                                </div>
                                            </div>
                                        </a>
                                    </div>

                                </div>
                            </div>
                                       <div class="tyn-reply-separator">${formatDate(message.createdDateTime)} </div>
                            `
                }
                else if (message.type == 3) //link
                {
                    html = `
                                <div class="tyn-reply-bubble outgoing">
                                    <div class="tyn-reply-link">
                                       ${message.message}
                                    </div>
                                </div>
                                           <div class="tyn-reply-separator">${formatDate(message.createdDateTime)} </div>

                            `;
                }
                else if (message.type == 5) //image
                {
                    html = `<div class="tab-content">
                                    <div class="tab-pane show active" id="chat-media-images" tabindex="0">

                                    <div class="row g-3">
                                       <div class="col-8"> </div>
                                            <div class="col-4">
                                                <a href="data:image/jpeg;base64,${message.file}" download class="glightbox tyn-thumb" data-gallery="media-photo">
                                                    <img src="data:image/jpeg;base64,${message.file}" class="tyn-image" alt="">
                                                </a>

                                            </div>

                                        </div>
                                    </div>
                                               <div class="tyn-reply-separator">${formatDate(message.createdDateTime)}</div>
                                    `
                }
            }
            else {
                const eyeIcon = message.isRead ? '<i class="fas fa-eye"></i>' : '';
                if (message.type == 1) {
                    html = `<div class="tyn-reply-item incoming">
            <div class="tyn-reply-avatar">
                <div class="tyn-media tyn-size-md tyn-circle">
                    <img src="/${message.senderUserProfileUrl}" alt="">
                </div>
            </div>
            <div class="tyn-reply-group">
                <div class="tyn-reply-bubble">

                    <div class="tyn-reply-text">${message.message} </div>
                </div>
            </div>
        </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)} </div>
        `
                }
                else if (message.type == 2) //file
                {
                    html = `<div class="tyn-reply-item incoming">
            <div class="tyn-reply-avatar">
                <div class="tyn-media tyn-size-md tyn-circle">
                    <img src="/${message.senderUserProfileUrl}" alt="">
                </div>
            </div>
             <div class="tyn-reply-bubble">
                                    <div class="tyn-reply-file">
                                        <a href="#" onclick="downloadBase64File('${message.file}', '${message.fileName}')"  class="tyn-file">
                                            <div class="tyn-media-group">
                                                <div class="tyn-media tyn-size-lg text-bg-light">
                                                    <!-- filetype-docx -->
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-filetype-docx" viewBox="0 0 16 16">
                                                        <path fill-rule="evenodd" d="M14 4.5V11h-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5zm-6.839 9.688v-.522a1.5 1.5 0 0 0-.117-.641.86.86 0 0 0-.322-.387.86.86 0 0 0-.469-.129.87.87 0 0 0-.471.13.87.87 0 0 0-.32.386 1.5 1.5 0 0 0-.117.641v.522q0 .384.117.641a.87.87 0 0 0 .32.387.9.9 0 0 0 .471.126.9.9 0 0 0 .469-.126.86.86 0 0 0 .322-.386 1.55 1.55 0 0 0 .117-.642m.803-.516v.513q0 .563-.205.973a1.47 1.47 0 0 1-.589.627q-.381.216-.917.216a1.86 1.86 0 0 1-.92-.216 1.46 1.46 0 0 1-.589-.627 2.15 2.15 0 0 1-.205-.973v-.513q0-.569.205-.975.205-.411.59-.627.386-.22.92-.22.535 0 .916.22.383.219.59.63.204.406.204.972M1 15.925v-3.999h1.459q.609 0 1.005.235.396.233.589.68.196.445.196 1.074 0 .634-.196 1.084-.197.451-.595.689-.396.237-.999.237zm1.354-3.354H1.79v2.707h.563q.277 0 .483-.082a.8.8 0 0 0 .334-.252q.132-.17.196-.422a2.3 2.3 0 0 0 .068-.592q0-.45-.118-.753a.9.9 0 0 0-.354-.454q-.237-.152-.61-.152Zm6.756 1.116q0-.373.103-.633a.87.87 0 0 1 .301-.398.8.8 0 0 1 .475-.138q.225 0 .398.097a.7.7 0 0 1 .273.26.85.85 0 0 1 .12.381h.765v-.073a1.33 1.33 0 0 0-.466-.964 1.4 1.4 0 0 0-.49-.272 1.8 1.8 0 0 0-.606-.097q-.534 0-.911.223-.375.222-.571.633-.197.41-.197.978v.498q0 .568.194.976.195.406.571.627.375.216.914.216.44 0 .785-.164t.551-.454a1.27 1.27 0 0 0 .226-.674v-.076h-.765a.8.8 0 0 1-.117.364.7.7 0 0 1-.273.248.9.9 0 0 1-.401.088.85.85 0 0 1-.478-.131.83.83 0 0 1-.298-.393 1.7 1.7 0 0 1-.103-.627zm5.092-1.76h.894l-1.275 2.006 1.254 1.992h-.908l-.85-1.415h-.035l-.852 1.415h-.862l1.24-2.015-1.228-1.984h.932l.832 1.439h.035z" />
                                                    </svg>
                                                </div>
                                                <div class="tyn-media-col">
                                                    <h6 class="name">${message.fileName}</h6>
                                                    <div class="meta">${message.size} MB</div>
                                                </div>
                                            </div>
                                        </a>
                                    </div>

                                </div>
                            </div>
            </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)}</div>`
                }
                else if (message.type == 3) //file
                {
                    html = `<div class="tyn-reply-item incoming">
            <div class="tyn-reply-avatar">
                <div class="tyn-media tyn-size-md tyn-circle">
                    <img src="/${message.senderUserProfileUrl}" alt="">
                </div>
            </div>
             <div class="tyn-reply-group">
                                <div class="tyn-reply-bubble">
                                    <div class="tyn-reply-link">
                                       ${message.message}
                                    </div>
                                </div><!-- .tyn-reply-bubble -->
             </div>
            </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)}</div>`
                }
                else if (message.type == 5) //image
                {
                    html = `<div class="tyn-reply-item incoming">
            <div class="tyn-reply-avatar">
                <div class="tyn-media tyn-size-md tyn-circle">
                    <img src="/${message.senderUserProfileUrl}" alt="">
                </div>
            </div>
            <div class="tyn-reply-group">
                <div class="tyn-reply-bubble">
                   <div class="col-4">
                    <a href="data:image/jpeg;base64,${message.file}" download class="glightbox tyn-thumb" data-gallery="media-photo">
                        <img src="data:image/jpeg;base64,${message.file}" class="tyn-image" alt="">
                    </a>
                   </div>
                </div>
            </div>
        </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)}</div>`
                }
            }
            $('#tynQuickReply').append(html);
        });
        $('#loader').css('display', 'none')
        $('#tynQuickReply').scrollTop($('#tynQuickReply')[0].scrollHeight);
    });
    chatconnection.on("ReceiveMessage", function (currentUserId, message) {
        const isCurrentUser = (currentUserId === message.senderId);
        console.log(isCurrentUser);
        const eyeIcon = message.isRead ? '<i class="fa fa-eye"></i>' : '';
        let html;

        debugger
        if (currentUserId === CurrentUserId) {
            // If it's from the current user, do nothing (already displayed)
            return false;
        }
        if (index <= 0) {
            if (message.type == 2) //file
            {
                ++index;
                html = `<div class="tyn-reply-item incoming">
            <div class="tyn-reply-avatar">
                <div class="tyn-media tyn-size-md tyn-circle">
                    <img src="/${message.senderUserProfileUrl}" alt="">
                </div>
            </div>
             <div class="tyn-reply-bubble">
                                    <div class="tyn-reply-file">
                                        <a href="#" onclick="downloadBase64File('${message.file}', '${message.fileName}')"  class="tyn-file">
                                            <div class="tyn-media-group">
                                                <div class="tyn-media tyn-size-lg text-bg-light">
                                                    <!-- filetype-docx -->
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-filetype-docx" viewBox="0 0 16 16">
                                                        <path fill-rule="evenodd" d="M14 4.5V11h-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5zm-6.839 9.688v-.522a1.5 1.5 0 0 0-.117-.641.86.86 0 0 0-.322-.387.86.86 0 0 0-.469-.129.87.87 0 0 0-.471.13.87.87 0 0 0-.32.386 1.5 1.5 0 0 0-.117.641v.522q0 .384.117.641a.87.87 0 0 0 .32.387.9.9 0 0 0 .471.126.9.9 0 0 0 .469-.126.86.86 0 0 0 .322-.386 1.55 1.55 0 0 0 .117-.642m.803-.516v.513q0 .563-.205.973a1.47 1.47 0 0 1-.589.627q-.381.216-.917.216a1.86 1.86 0 0 1-.92-.216 1.46 1.46 0 0 1-.589-.627 2.15 2.15 0 0 1-.205-.973v-.513q0-.569.205-.975.205-.411.59-.627.386-.22.92-.22.535 0 .916.22.383.219.59.63.204.406.204.972M1 15.925v-3.999h1.459q.609 0 1.005.235.396.233.589.68.196.445.196 1.074 0 .634-.196 1.084-.197.451-.595.689-.396.237-.999.237zm1.354-3.354H1.79v2.707h.563q.277 0 .483-.082a.8.8 0 0 0 .334-.252q.132-.17.196-.422a2.3 2.3 0 0 0 .068-.592q0-.45-.118-.753a.9.9 0 0 0-.354-.454q-.237-.152-.61-.152Zm6.756 1.116q0-.373.103-.633a.87.87 0 0 1 .301-.398.8.8 0 0 1 .475-.138q.225 0 .398.097a.7.7 0 0 1 .273.26.85.85 0 0 1 .12.381h.765v-.073a1.33 1.33 0 0 0-.466-.964 1.4 1.4 0 0 0-.49-.272 1.8 1.8 0 0 0-.606-.097q-.534 0-.911.223-.375.222-.571.633-.197.41-.197.978v.498q0 .568.194.976.195.406.571.627.375.216.914.216.44 0 .785-.164t.551-.454a1.27 1.27 0 0 0 .226-.674v-.076h-.765a.8.8 0 0 1-.117.364.7.7 0 0 1-.273.248.9.9 0 0 1-.401.088.85.85 0 0 1-.478-.131.83.83 0 0 1-.298-.393 1.7 1.7 0 0 1-.103-.627zm5.092-1.76h.894l-1.275 2.006 1.254 1.992h-.908l-.85-1.415h-.035l-.852 1.415h-.862l1.24-2.015-1.228-1.984h.932l.832 1.439h.035z" />
                                                    </svg>
                                                </div>
                                                <div class="tyn-media-col">
                                                    <h6 class="name">${message.fileName}</h6>
                                                    <div class="meta">${message.size} MB</div>
                                                </div>
                                            </div>
                                        </a>
                                    </div>

                                </div>
                            </div>
            </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)} </div>`
            }
            else if (message.type == 3) //link
            {
                html = `<div class="tyn-reply-item incoming">
            <div class="tyn-reply-avatar">
                <div class="tyn-media tyn-size-md tyn-circle">
                    <img src="/${message.senderUserProfileUrl}" alt="">
                </div>
            </div>
             <div class="tyn-reply-group">
                                <div class="tyn-reply-bubble">
                                    <div class="tyn-reply-link">
                                       ${message.message}
                                    </div>
                                </div><!-- .tyn-reply-bubble -->
             </div>
            </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)}</div>`
            }
            else if (message.type == 5) //image
            {
                html = `<div class="tyn-reply-item incoming">
            <div class="tyn-reply-avatar">
                <div class="tyn-media tyn-size-md tyn-circle">
                    <img src="/${message.senderUserProfileUrl}" alt="">
                </div>
            </div>
            <div class="tyn-reply-group">
                <div class="tyn-reply-bubble">
                   <div class="col-4">
                    <a href="data:image/jpeg;base64,${message.file}" download class="glightbox tyn-thumb" data-gallery="media-photo">
                        <img src="data:image/jpeg;base64,${message.file}" class="tyn-image" alt="">
                    </a>
                   </div>
                </div>
            </div>
        </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)}</div>`
            }
            else {
                html = `<div class="tyn-reply-item incoming">
            <div class="tyn-reply-avatar">
                <div class="tyn-media tyn-size-md tyn-circle">
                    <img src="/${message.senderUserProfileUrl}" alt="">
                </div>
            </div>
            <div class="tyn-reply-group">
                <div class="tyn-reply-bubble">

                    <div class="tyn-reply-text">${message.message} </div>
                </div>
            </div>
        </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)}</div>`
            }
        }

        $('#tynQuickReply').prepend(html);
    });
    chatconnection.on("SendMessage", function (currentUserId, message) {
        const isCurrentUser = (currentUserId === message.senderId);
        console.log(isCurrentUser);
        const eyeIcon = message.isRead ? '<i class="fa fa-eye"></i>' : '';
        let html;
        if (isCurrentUser) {
            if (message.type == 1) {
                html = `<div class="tyn-reply-item outgoing">
                <div class="tyn-reply-group">
                    <div class="tyn-reply-bubble">

                        <div class="tyn-reply-text"> ${message.message} </div>
                    </div>
                </div>
            </div>
                <div class="tyn-reply-separator">${formatDate(message.createdDateTime)} </div>
            `
            }
            else if (message.type == 2) //file
            {
                html = ` <div class="tyn-reply-bubble outgoing">
                                    <div class="tyn-reply-file">
                                        <a href="#" onclick="downloadBase64File('${message.file}', '${message.fileName}')"  class="tyn-file">
                                            <div class="tyn-media-group">
                                                <div class="tyn-media tyn-size-lg text-bg-light">
                                                    <!-- filetype-docx -->
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-filetype-docx" viewBox="0 0 16 16">
                                                        <path fill-rule="evenodd" d="M14 4.5V11h-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5zm-6.839 9.688v-.522a1.5 1.5 0 0 0-.117-.641.86.86 0 0 0-.322-.387.86.86 0 0 0-.469-.129.87.87 0 0 0-.471.13.87.87 0 0 0-.32.386 1.5 1.5 0 0 0-.117.641v.522q0 .384.117.641a.87.87 0 0 0 .32.387.9.9 0 0 0 .471.126.9.9 0 0 0 .469-.126.86.86 0 0 0 .322-.386 1.55 1.55 0 0 0 .117-.642m.803-.516v.513q0 .563-.205.973a1.47 1.47 0 0 1-.589.627q-.381.216-.917.216a1.86 1.86 0 0 1-.92-.216 1.46 1.46 0 0 1-.589-.627 2.15 2.15 0 0 1-.205-.973v-.513q0-.569.205-.975.205-.411.59-.627.386-.22.92-.22.535 0 .916.22.383.219.59.63.204.406.204.972M1 15.925v-3.999h1.459q.609 0 1.005.235.396.233.589.68.196.445.196 1.074 0 .634-.196 1.084-.197.451-.595.689-.396.237-.999.237zm1.354-3.354H1.79v2.707h.563q.277 0 .483-.082a.8.8 0 0 0 .334-.252q.132-.17.196-.422a2.3 2.3 0 0 0 .068-.592q0-.45-.118-.753a.9.9 0 0 0-.354-.454q-.237-.152-.61-.152Zm6.756 1.116q0-.373.103-.633a.87.87 0 0 1 .301-.398.8.8 0 0 1 .475-.138q.225 0 .398.097a.7.7 0 0 1 .273.26.85.85 0 0 1 .12.381h.765v-.073a1.33 1.33 0 0 0-.466-.964 1.4 1.4 0 0 0-.49-.272 1.8 1.8 0 0 0-.606-.097q-.534 0-.911.223-.375.222-.571.633-.197.41-.197.978v.498q0 .568.194.976.195.406.571.627.375.216.914.216.44 0 .785-.164t.551-.454a1.27 1.27 0 0 0 .226-.674v-.076h-.765a.8.8 0 0 1-.117.364.7.7 0 0 1-.273.248.9.9 0 0 1-.401.088.85.85 0 0 1-.478-.131.83.83 0 0 1-.298-.393 1.7 1.7 0 0 1-.103-.627zm5.092-1.76h.894l-1.275 2.006 1.254 1.992h-.908l-.85-1.415h-.035l-.852 1.415h-.862l1.24-2.015-1.228-1.984h.932l.832 1.439h.035z" />
                                                    </svg>
                                                </div>
                                                <div class="tyn-media-col">
                                                    <h6 class="name">${message.fileName}</h6>
                                                    <div class="meta">${message.size} MB</div>
                                                </div>
                                            </div>
                                        </a>
                                    </div>

                                </div>
                            </div>
                            <div class="tyn-reply-separator">${formatDate(message.createdDateTime)}</div>
                            `
            }
            else if (message.type == 3) //link
            {
                html = `
                                <div class="tyn-reply-bubble outgoing">
                                    <div class="tyn-reply-link">
                                       ${message.message}
                                    </div>
                                </div>
                                           <div class="tyn-reply-separator">${formatDate(message.createdDateTime)} </div>

                            `;
            }
            else if (message.type == 5) //image
            {
                html = `<div class="tab-content">
                                    <div class="tab-pane show active" id="chat-media-images" tabindex="0">

                                    <div class="row g-3">
                                       <div class="col-8"> </div>
                                            <div class="col-4">
                                                <a href="data:image/jpeg;base64,${message.file}" download class="glightbox tyn-thumb" data-gallery="media-photo">
                                                    <img src="data:image/jpeg;base64,${message.file}" class="tyn-image" alt="">
                                                </a>

                                            </div>

                                        </div>
                                    </div>
                                    <div class="tyn-reply-separator">${formatDate(message.createdDateTime)} </div>
                                   `
            }
        }

        $('#tynQuickReply').prepend(html);
    });
    chatconnection.on('UserTyping', function (user) {
        $('#type').css('display', 'block');
    });
    chatconnection.on('UserStoppedTyping', function (user) {
        $('#type').css('display', 'none');
    });
}
function getFiles() {
    showLoader();
    $.ajax({
        url: '/Chat/GetMediaFile?groupName=' + GroupName,
        type: 'Get',
        dataType: "json",
        success: function (response) {
            $('#images').empty();
            $('#files').empty();
            $.each(response, function (i, v) {
                let html = `  <li>
                            <a href="#" onclick="downloadBase64File('${v.fileBase64}', '${v.name}')" class="tyn-file">
                                <div class="tyn-media-group">
                                    <div class="tyn-media tyn-size-lg text-bg-light">
                                        <!-- filetype-docx -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-filetype-docx" viewBox="0 0 16 16">
                                            <path fill-rule="evenodd" d="M14 4.5V11h-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5zm-6.839 9.688v-.522a1.5 1.5 0 0 0-.117-.641.86.86 0 0 0-.322-.387.86.86 0 0 0-.469-.129.87.87 0 0 0-.471.13.87.87 0 0 0-.32.386 1.5 1.5 0 0 0-.117.641v.522q0 .384.117.641a.87.87 0 0 0 .32.387.9.9 0 0 0 .471.126.9.9 0 0 0 .469-.126.86.86 0 0 0 .322-.386 1.55 1.55 0 0 0 .117-.642m.803-.516v.513q0 .563-.205.973a1.47 1.47 0 0 1-.589.627q-.381.216-.917.216a1.86 1.86 0 0 1-.92-.216 1.46 1.46 0 0 1-.589-.627 2.15 2.15 0 0 1-.205-.973v-.513q0-.569.205-.975.205-.411.59-.627.386-.22.92-.22.535 0 .916.22.383.219.59.63.204.406.204.972M1 15.925v-3.999h1.459q.609 0 1.005.235.396.233.589.68.196.445.196 1.074 0 .634-.196 1.084-.197.451-.595.689-.396.237-.999.237zm1.354-3.354H1.79v2.707h.563q.277 0 .483-.082a.8.8 0 0 0 .334-.252q.132-.17.196-.422a2.3 2.3 0 0 0 .068-.592q0-.45-.118-.753a.9.9 0 0 0-.354-.454q-.237-.152-.61-.152Zm6.756 1.116q0-.373.103-.633a.87.87 0 0 1 .301-.398.8.8 0 0 1 .475-.138q.225 0 .398.097a.7.7 0 0 1 .273.26.85.85 0 0 1 .12.381h.765v-.073a1.33 1.33 0 0 0-.466-.964 1.4 1.4 0 0 0-.49-.272 1.8 1.8 0 0 0-.606-.097q-.534 0-.911.223-.375.222-.571.633-.197.41-.197.978v.498q0 .568.194.976.195.406.571.627.375.216.914.216.44 0 .785-.164t.551-.454a1.27 1.27 0 0 0 .226-.674v-.076h-.765a.8.8 0 0 1-.117.364.7.7 0 0 1-.273.248.9.9 0 0 1-.401.088.85.85 0 0 1-.478-.131.83.83 0 0 1-.298-.393 1.7 1.7 0 0 1-.103-.627zm5.092-1.76h.894l-1.275 2.006 1.254 1.992h-.908l-.85-1.415h-.035l-.852 1.415h-.862l1.24-2.015-1.228-1.984h.932l.832 1.439h.035z" />
                                        </svg>
                                    </div>
                                    <div class="tyn-media-col">
                                        <h6 class="name">${v.name}</h6>
                                        <div class="meta">${v.size}</div>
                                    </div>
                                </div>
                            </a>
                          </li>`

                $('#files').append(html)
            })
        },
        error: function (xhr, status, error) {
            console.error("Error uploading image: " + error);
        }, complete: function () {
            hideLoader();
        }
    });
}
function getImages() {
    showLoader();
    $.ajax({
        url: '/Chat/GetMediaImages?groupName=' + GroupName,
        type: 'Get',
        dataType: "json",
        success: function (response) {
            $('#images').empty();

            $.each(response, function (i, v) {
                let html = ` <div class="col-4">
                                <a href="data:image/jpeg;base64,${v.file}" download class="glightbox tyn-thumb" data-gallery="media-photo">
                                    <img src="data:image/jpeg;base64,${v.file}" class="tyn-image" alt="">
                                </a>
                             </div>`

                $('#images').append(html)
            })
        },
        error: function (xhr, status, error) {
            console.error("Error uploading image: " + error);
        }, complete: function () {
            hideLoader();
        }
    });
}
function getVidoes() {
    showLoader();
    $.ajax({
        url: '/Chat/GetMediaVideos?groupName=' + GroupName,
        type: 'Get',
        dataType: "json",
        success: function (response) {
            $('#images').empty();
            $('#videos').empty();

            $.each(response, function (i, v) {
                let html = `<div class="col-6">
                            <video width="320" height="240" controls>
                            <source src="data:video/mp4;base64,${v.file}" type="video/mp4">
                            Your browser does not support the video tag.
                           </video>
                           </div>`

                $('#videos').append(html)
            })
        },
        error: function (xhr, status, error) {
            console.error("Error uploading image: " + error);
        }, complete: function () {
            hideLoader();
        }
    });
}
function getLinks() {
    showLoader();
    $.ajax({
        url: '/Chat/GetMediaLinks?groupName=' + GroupName,
        type: 'Get',
        dataType: "json",
        success: function (response) {
            $('#images').empty();
            $('#links').empty();

            $.each(response, function (i, v) {
                let html = `<li>
                           ${v.url}

                            </a>
                            </li>`

                $('#links').append(html)
            })
        },
        error: function (xhr, status, error) {
            console.error("Error uploading image: " + error);
        }, complete: function () {
            hideLoader();
        }
    });
}
function downloadBase64File(base64Data, fileName) {
    // Convert base64 to binary
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);

    // Create blob from binary data
    const blob = new Blob([byteArray], { type: 'application/octet-stream' });

    // Create download link
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
}
function formatDate(dateString) {
    const date = new Date(dateString);
    const options = {
        year: 'numeric', month: 'long', day: 'numeric',
        hour: 'numeric', minute: 'numeric',
        hour12: true
    };
    return date.toLocaleString('en-US', options);
}
function stopHubConnection() {
    chatconnection.stop();
}
function sendMessage() {
    //const chatconnection = new signalR.HubConnectionBuilder().withUrl(`/ chat`).configureLogging(signalR.LogLevel.Information).build();
    let tutorId = otherUserId;
    let div = document.getElementById('tynChatInput');

    let chat = {};
    chat.Message = div.textContent;
    chat.RecipientId = tutorId;
    chat.GroupName = GroupName;
    chatconnection.invoke("SendMessage", chat).catch(function (err) {
        showToast('error', 'Server Error!')
    });
}
function sendFile(file, name, size, extension) {
    // Create an object to hold the file and additional data
    let chat = {};
    chat.Base64File = file;
    chat.RecipientId = otherUserId;
    chat.FileName = name;
    chat.Size = size.toString();
    chat.Extension = extension;
    chat.GroupName = GroupName;

    chatconnection.invoke("SendFile", chat).catch(function (err) {
        showToast('error', err.toString())
    });
}

function formatFileSize(bytes) {
    if (bytes >= 1024 * 1024) {
        return Math.round(bytes / (1024 * 1024)) + ' MB';
    } else if (bytes >= 1024) {
        return Math.round(bytes / 1024) + ' KB';
    } else {
        return bytes + ' bytes';
    }
}
function scrollToBottom() {
    $('#tynQuickReply').scrollTop($('#tynQuickReply')[0].scrollHeight);
}
window.onload = function () {
    scrollToBottom()
    var button = document.getElementById("first-recepient");
    if (button) {
        button.click();
    }
    $('#tynChatInput').on('blur', function () {
        chatconnection.invoke('StopTyping', otherUserId).catch(function (err) {
            return console.error(err.toString());
        });
    });
    let typingTimeout;
    const TYPING_TIMER_LENGTH = 3000; // 3 seconds

    $('#tynChatInput').on('input', function () {
        chatconnection.invoke('Typing', otherUserId).catch(function (err) {
            return console.error(err.toString());
        });

        clearTimeout(typingTimeout);
        typingTimeout = setTimeout(function () {
            chatconnection.invoke('StopTyping', otherUserId).catch(function (err) {
                return console.error(err.toString());
            });
        }, TYPING_TIMER_LENGTH);
    });
};

document.getElementById('uploadFile').addEventListener('click', function (event) {
    event.preventDefault();
    document.getElementById('fileInput').click();
});
document.getElementById('fileInput').addEventListener('change', function (event) {
    const file = event.target.files[0];
    if (file) {
        const fileExtension = file.name.split('.').pop();
        let chat = {};
        const fileSizeInBytes = file.size;
        const maxFileSizeInBytes = 1 * 1024 * 1024; // 1 MB

        if (fileSizeInBytes > maxFileSizeInBytes) {
            showToast("error", "File size exceeds 1 MB.");
            return;
        }
        const fileSizeInKB = formatFileSize(fileSizeInBytes);

        const reader = new FileReader();
        reader.onload = function (loadEvent) { // Using a different variable name here
            // Convert the file to Base64
            const base64Image = loadEvent.target.result.split(",")[1];

            // Send the Base64 encoded image data via SignalR
            sendFile(base64Image, file.name, fileSizeInKB, fileExtension);
        };
        reader.readAsDataURL(file);
    }
});