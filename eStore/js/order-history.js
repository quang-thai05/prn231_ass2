var token = localStorage.getItem("token");
var decoded;
var userId;

$(document).ready(() => {
	if (token != null) {
		decoded = jwt_decode(token);
        userId = decoded["UserId"]

        $.ajax ({
            url: "https://localhost:7135/api/Order/GetOrdersByUserId/" + userId,
            method: "GET",
            contentType: "application/json",
            headers: {
                Authorization: "Bearer " + token
            },
            success: (response) => {
                $("#order-table").append(
                    response.map((order) => (
                        `<tr>
                            <th scope="row">${order.orderId}</th>
                            <td>${order.orderDate}</td>
                            <td>${order.requiredDate}</td>
                            <td>${order.requiredDate}</td>
                            <td>${order.freight}</td>
                        </tr>`
                    ))
                )
            },
            error: (xhr) => {
                SlimNotifierJs.notification(
                    "error",
                    "Error",
                    xhr.responseText,
                    3000
                );
            }
        })
	}
});
