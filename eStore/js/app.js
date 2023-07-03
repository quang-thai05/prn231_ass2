var token = localStorage.getItem("token");
var decoded;
var userRole;

$(document).ready(() => {
	if (token != null) {
		decoded = jwt_decode(token);
		userRole =
			decoded[
				"http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
			];

		if (userRole == "Admin") {
			$("#add-prod-button").append(
				`<button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#addProductModal">
					Add Product
				</button>`
			);
		}

		$("#add-prod-btn").click(() => {
			let product = {
				categoryId: $("#category-name").val(),
				productName: $("#product-name").val(),
				weight: $("#product-weight").val(),
				unitPrice: $("#product-unit-price").val(),
				unitInStock: $("#product-unit-in-stock").val(),
			};
			$.ajax({
				url: "https://localhost:7135/api/Product/AddProduct",
				method: "POST",
				contentType: "application/json",
				data: JSON.stringify(product),
				headers: {
					Authorization: "Bearer " + token,
				},
				success: (response) => {
					SlimNotifierJs.notification(
						"success",
						"Added Successfully!",
						response,
						3000
					);
					loadProducts(userRole);
					$("#category-name").val("");
					$("#product-name").val("");
					$("#product-weight").val("");
					$("#product-unit-price").val("");
					$("#product-unit-in-stock").val("");
				},
				error: function (xhr, status, error) {
					SlimNotifierJs.notification(
						"error",
						"Error",
						xhr.responseText,
						3000
					);
				},
			});
		});

		loadProducts(userRole);
	} else {
		loadProducts("");
	}

	$.ajax({
		url: "https://localhost:7135/api/Category/GetAllCate",
		method: "GET",
		contentType: "application/json",
		success: (response) => {
			$("#category-name").append(
				response.map(
					(cat) =>
						`<option value="${cat.categoryId}">${cat.categoryName}</option>`
				)
			);
			$("#category-name-update").append(
				response.map(
					(cat) =>
						`<option value="${cat.categoryId}">${cat.categoryName}</option>`
				)
			);
		},
		error: function (xhr, status, error) {
			SlimNotifierJs.notification(
				"error",
				"Error",
				xhr.responseText,
				3000
			);
		},
	});
});

function loadProducts(role) {
	$.ajax({
		url: "https://localhost:7135/api/Product/GetAllProducts",
		method: "GET",
		contentType: "application/json",
		success: (response) => {
			$("#product-table").empty();
			if (role === "Admin") {
				$("#product-table").append(
					response.map(
						(product) =>
							`<tr>
								<th scope="row">${product.productId}</th>
								<td>${product.category}</td>
								<td>${product.productName}</td>
								<td>${product.weight}</td>
								<td>${product.unitPrice}$</td>
								<td>${product.unitInStock}</td>
								<td>
									<button type="button" class="btn btn-success" data-bs-toggle="modal"
											data-bs-target="#updateProductModal"
											data-prod-id="${product.productId}"
											data-prod-cate="${product.categoryId}"
											data-prod-name="${product.productName}"
											data-prod-weight="${product.weight}"
											data-prod-unit-price="${product.unitPrice}"
											data-prod-unit-in-stock="${product.unitInStock}"
											onclick="updateProd(this)">
										<i class="fa-solid fa-pen-to-square"></i>
									</button>
									<button type="button" class="btn btn-danger" data-prod-id="${product.productId}"
											onclick="deleteProd(this)">
										<i class="fa-solid fa-trash"></i>
									</button>
								</td>
							</tr>`
					)
				);
			} else {
				$("#product-table").append(
					response.map(
						(product) => (
							`<tr>
								<th scope="row">${product.productId}</th>
								<td>${product.category}</td>
								<td>${product.productName}</td>
								<td>${product.weight}</td>
								<td>${product.unitPrice}$</td>
								<td>${product.unitInStock}</td>
								<td>
									<button type="button" class="btn btn-primary"
										data-prod-id="${product.productId}"
										onclick="addToCart(this)">
										<i class="fa-solid fa-cart-shopping"></i>
									</button>
								</td>
							</tr>`
						)
					)
				);
			}
		},
		error: function (xhr, status, error) {
			SlimNotifierJs.notification(
				"error",
				"Error",
				xhr.responseText,
				3000
			);
		},
	});
}

function addToCart(model) {
	let data = {
		productId: model.getAttribute("data-prod-id"),
		quantity: 1,
	}

	$.ajax({
		url: "https://localhost:7135/api/Cart/AddToCart",
		method: "POST",
		credentials: 'include',
		contentType: "application/json",
		data: JSON.stringify(data),
		headers: {
			Authorization: "Bearer " + token,
		},
		success: (res) => {
			SlimNotifierJs.notification(
				"success",
				"Success",
				res,
				3000
			);
			loadProducts();
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

var prodId;
var updatedProduct;
var currentProduct;

function updateProd(model) {
	currentProduct = {
		categoryId: model.getAttribute("data-prod-cate"),
		productName: model.getAttribute("data-prod-name"),
		weight: model.getAttribute("data-prod-weight"),
		unitPrice: model.getAttribute("data-prod-unit-price"),
		unitInStock: model.getAttribute("data-prod-unit-in-stock"),
	};

	$("#category-name-update").val(currentProduct.categoryId);
	$("#product-name-update").val(currentProduct.productName);
	$("#product-weight-update").val(currentProduct.weight);
	$("#product-unit-price-update").val(currentProduct.unitPrice);
	$("#product-unit-in-stock-update").val(currentProduct.unitInStock);

	prodId = model.getAttribute("data-prod-id");
}

function deleteProd(model) {
	prodId = model.getAttribute("data-prod-id");
	$.ajax({
		url: "https://localhost:7135/api/Product/DeleteProduct/" + prodId,
		method: "DELETE",
		contentType: "application/json",
		headers: {
			Authorization: "Bearer " + token,
		},
		success: (response) => {
			SlimNotifierJs.notification(
				"success",
				"Updated Successfully!",
				response,
				3000
			);
			loadProducts(userRole);
		},
		error: function (xhr, status, error) {
			SlimNotifierJs.notification(
				"error",
				"Error",
				xhr.responseText,
				3000
			);
		},
	});
}

$("#update-prod-btn").click(() => {
	updatedProduct = {
		categoryId: $("#category-name-update").val(),
		productName: $("#product-name-update").val(),
		weight: $("#product-weight-update").val(),
		unitPrice: $("#product-unit-price-update").val(),
		unitInStock: $("#product-unit-in-stock-update").val(),
	};

	$.ajax({
		url: "https://localhost:7135/api/Product/UpdateProduct/" + prodId,
		method: "PUT",
		contentType: "application/json",
		data: JSON.stringify(updatedProduct),
		headers: {
			Authorization: "Bearer " + token,
		},
		success: (response) => {
			SlimNotifierJs.notification(
				"success",
				"Updated Successfully!",
				response,
				3000
			);
			loadProducts(userRole);
		},
		error: function (xhr, status, error) {
			SlimNotifierJs.notification(
				"error",
				"Error",
				xhr.responseText,
				3000
			);
		},
	});
});
