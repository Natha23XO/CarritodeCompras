$(document).ready(function () {
    jQuery.ajax({
        url: "/Tienda/ObtenerCarrito",
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $.LoadingOverlay("hide");
            if (data.lista != null) {
                $.each(data.lista, function (i, item) {
                    $("<div>").addClass("producto mb-2").append(
                        $("<div>").addClass("producto-body").append(
                            $("<div>").addClass("row align-items-center").append(
                                $("<div>").addClass("col-md-3").append(
                                    $("<img>").addClass("").attr({ "src": "data:image/" + item.oProducto.extension + ";base64," + item.oProducto.base64, "width": "200" })
                                ),
                                $("<div>").addClass("col-md-4").append(
                                    $("<div>").addClass("ml-2").append(
                                        $("<span>").addClass("font-weight-bold d-block").text(item.oProducto.oMarca.descripcion),
                                        $("<span>").addClass("spec overflow-hidden").text(item.oProducto.nombre),
                                    )
                                ),
                                $("<div>").addClass("col-md-3").append(
                                    $("<div>").addClass("d-flex").append(
                                        $("<div>").addClass("d-flex controles").append(
                                            $("<button>").addClass("btn btn btn-dark rounded-pill px-3 me-2 btn-restar").append($("<i>").addClass("fas fa-minus")).attr({ "type": "button" }),
                                            $("<input>").addClass("form-control input-cantidad p-1 text-center rounded-pill input-cantidad p-1").css({ "width": "40px" }).attr({ "disabled": "disabled" }).val("1").data("precio", item.oProducto.precio).data("idproducto", item.oProducto.idProducto),
                                            $("<button>").addClass("btn btn btn-dark rounded-pill px-3 ms-2 btn-sumar").append($("<i>").addClass("fas fa-plus")).attr({ "type": "button" })
                                        )
                                    )
                                ),
                                $("<div>").addClass("col-md-2 d-flex flex-column align-items-end").append(
                                    $("<span>").addClass("mb-4").text("S./" + item.oProducto.precio),
                                    $("<a>")
                                        .addClass("btn-eliminar")
                                        .text("Eliminar")
                                        .data("informacion", { _IdCarrito: item.idCarrito, _IdProducto: item.oProducto.idProducto })
                                        .on({
                                            mouseenter: function () {
                                                // Al pasar el mouse, agregar estilo
                                                $(this).css({
                                                    textDecoration: "underline"
                                                });
                                            },
                                            mouseleave: function () {
                                                // Al salir del mouse, quitar estilo
                                                $(this).css({
                                                    textDecoration: "none"
                                                });
                                            }
                                        })
                                        .css({
                                            cursor: "pointer"
                                        }))
                            ),
                            $("<hr>") // Línea separadora
                        )
                    ).appendTo("#productos-seleccionados");
                });
                obtenerPreciosPago();
                obtenerCantidadProductos();
            }
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {
            $.LoadingOverlay("show");
        },
    });
    ListarDepartamento();
})

$(document).on('click', '.btn-sumar', function (event) {
    var div = $(this).parent("div.controles");
    var input = $(div).find("input.input-cantidad");
    var cantidad = parseInt($(input).val()) + 1;
    $(input).val(cantidad);
    obtenerPreciosPago()
});

$(document).on('click', '.btn-restar', function (event) {
    var div = $(this).parent("div.controles");
    var input = $(div).find("input.input-cantidad");
    var cantidad = parseInt($(input).val()) - 1;
    if (cantidad >= 1) {
        $(input).val(cantidad);
    }
    obtenerPreciosPago()
});


$(document).on('click', '.btn-eliminar', function (event) {
    var json = $(this).data("informacion");
    //var idProducto =
    var card_producto = $(this).parents("div.producto");


    jQuery.ajax({
        url: "/Tienda/EliminarCarrito" + '?IdCarrito=' + json._IdCarrito + '&IdProducto=' + json._IdProducto,
        //url: '@Url.Action("EliminarCarrito", "Tienda")' + '?IdCarrito=' + json._IdCarrito + '&IdProducto=' + json._IdProducto,
        type: "POST",
        //data: { IdCarrito: json._IdCarrito, IdProducto: json._IdProducto },
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.resultado) {
                card_producto.remove();
                obtenerPreciosPago();
                obtenerCantidadProductos();
                obtenerCantidad();
            } else {
                alert("No se pudo eliminar")
            }
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {

        },
    });

})


function obtenerPreciosPago() {

    var total = 0;
    $("input.input-cantidad").each(function (index) {
        var precio = parseFloat($(this).val()) * parseFloat($(this).data("precio"));
        total = total + precio;
    });
    $("#totalPagar").text("S/. " + total.toString());
    $("#totalPagar1").text("S/. " + total.toString());
    $("#totalPagar2").text("S/. " + total.toString());
    $("#totalPagar3").text("S/. " + total.toString());
}
function obtenerCantidadProductos() {
    $("#cantidad-articulos").text(" " + $("#productos-seleccionados > div.card").length.toString() + " ");

    if ($("#productos-seleccionados > div.producto").length === 0) {
        $("#btnProcesarPago").prop("disabled", true);
    }

    if ($("#productos-seleccionados > div.producto").length === 0) {
        $("#btnContinuarCompra").prop("disabled", true);
    }
}

$("#btnContinuarCompra").on("click", function (e) {

    $("#mdContinuarCompra").modal("show");

})

$("#btnContinuarCompra1").on("click", function (e) {

    $("#mdContinuarCompra").modal("show");

})

$("#btnProcesarPago").on("click", function (e) {

    $("#mdContinuarCompra").modal("hide");
    $("#mdprocesopago").modal("show");

})

$("#btnProcesarPago").on("click", function (e) {
    $("#cboDepartamento").val($("#cboDepartamento option:first").val());
    $("#cboProvincia").val($("#cboProvincia option:first").val());
    $("#cboDistrito").val($("#cboDistrito option:first").val());
    $("#txtContacto").val("");
    $("#txtTelefono").val("");
    $("#txtDireccion").val("");
    $(".control-validar").removeClass("border border-danger");


    if ($("#trj_nombre").val().trim() == "") {
        $("#mensaje-error").text("Debe ingresar nombre del titular");
        $('#toast-alerta').toast('show');
        return;
    } else if ($("#trj_numero").val().trim() == "") {
        $("#mensaje-error").text("Debe ingresar número de la tarjeta");
        $('#toast-alerta').toast('show');
        return;
    } else if ($("#trj_vigencia").val().trim() == "") {
        $("#mensaje-error").text("Debe ingresar vigencia de la tarjeta");
        $('#toast-alerta').toast('show');
        return;
    } else if ($("#trj_cvv").val().trim() == "") {
        $("#mensaje-error").text("Debe ingresar CVV de la tarjeta");
        $('#toast-alerta').toast('show');
        return;
    }

    $("#mdprocesopago").modal("show");

})

function ListarDepartamento() {
    jQuery.ajax({
        url: "/Tienda/ObtenerDepartamento",
        type: "POST",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("<option>").attr({ "value": "00", "disabled": "disabled", "selected": "true" }).text("Seleccionar").appendTo("#cboDepartamento");
            if (data.lista != null) {
                $.each(data.lista, function (i, v) {
                    $("<option>").attr({ "value": v.idDepartamento }).text(v.descripcion).appendTo("#cboDepartamento");
                });
            }
            ListarProvincia();
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {
        },
    });
}

$("#cboDepartamento").on("change", function () {
    ListarProvincia();
});

function ListarProvincia() {
    jQuery.ajax({
        url: "/Tienda/ObtenerProvincia" + '?_IdDepartamento=' + $("#cboDepartamento").val(),
        //url: '@Url.Action("ObtenerProvincia", "Tienda")' + '?_IdDepartamento=' + $("#cboDepartamento").val(),
        type: "POST",
        //data: JSON.stringify({ _IdDepartamento: $("#cboDepartamento option:selected").val() }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cboProvincia").html("");
            $("<option>").attr({ "value": "00", "disabled": "disabled", "selected": "true" }).text("Seleccionar").appendTo("#cboProvincia");
            if (data.lista != null) {
                $.each(data.lista, function (i, v) {
                    $("<option>").attr({ "value": v.idProvincia }).text(v.descripcion).appendTo("#cboProvincia");
                });
            }
            ListarDistrito();
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {
        },
    });
}

$("#cboProvincia").on("change", function () {
    ListarDistrito();
});

function ListarDistrito() {
    jQuery.ajax({
        url: "/Tienda/ObtenerDistrito" + '?_IdProvincia=' + $("#cboProvincia").val() + '&_IdDepartamento=' + $("#cboDepartamento").val(),
        //url: '@Url.Action("ObtenerDistrito", "Tienda")' + '?_IdProvincia=' + $("#cboProvincia").val() + '&_IdDepartamento=' + $("#cboDepartamento").val(),
        type: "POST",
        //data: JSON.stringify({ _IdProvincia: $("#cboProvincia option:selected").val(), _IdDepartamento: $("#cboDepartamento option:selected").val() }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#cboDistrito").html("");
            $("<option>").attr({ "value": "00", "disabled": "disabled", "selected": "true" }).text("Seleccionar").appendTo("#cboDistrito");
            if (data.lista != null) {
                $.each(data.lista, function (i, v) {
                    $("<option>").attr({ "value": v.idProvincia }).text(v.descripcion).appendTo("#cboDistrito");
                });
            }
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {
        },
    });
}

$("#btnConfirmarCompra").on("click", function (e) {

    var falta_ingresar_datos = false;

    $(".control-validar").removeClass("border border-danger");

    $(".control-validar").each(function (i) {
        if ($(this).is('input')) {
            if ($(this).val() == "") {
                $(this).addClass("border border-danger")
                falta_ingresar_datos = true;
            }
        } else if ($(this).is('select')) {
            if ($(this).children("option:selected").val() == "00") {
                $(this).addClass("border border-danger")
                falta_ingresar_datos = true;
            }
        }
    });


    function AbrirModalConfirmarVenta() {
        const section = document.querySelector("section"),
            overlay = document.querySelector(".overlay"),
            showBtn = document.querySelector(".show-modal"),
            closeBtn = document.querySelector(".close-btn");

        section.classList.add("active");

    }

    if (!falta_ingresar_datos) {

        var detalle = [];
        var total = 0;
        $("input.input-cantidad").each(function (index) {
            var precio = parseFloat($(this).val()) * parseFloat($(this).data("precio"));
            detalle.push({
                IdProducto: parseInt($(this).data("idproducto")),
                Cantidad: parseInt($(this).val()),
                Total: precio
            });

            total = total + precio;
        });

        var objeto = {
            TotalProducto: $("#productos-seleccionados > div.card").length,
            Total: total,
            Contacto: $("#txtContacto").val(),
            Telefono: $("#txtTelefono").val(),
            Direccion: $("#txtDireccion").val(),
            IdDistrito: $("#cboDistrito").val(),
            oDetalleCompra: detalle
        }
        var request = new FormData();
        request.append("objeto", JSON.stringify(objeto));

        var tiendaUrl = '@Url.Action("Index", "Tienda")';
        jQuery.ajax({
            url: "/Tienda/RegistrarCompra",
            type: "POST",
            data: request,
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.resultado) {
                    $("#mdprocesopago").modal("hide");
                    AbrirModalConfirmarVenta()
                } else {
                    swal("Lo sentimos", "No se pudo completar la compra", "warning");
                }
            },
            error: function (error) {
                console.log(error)
            },
            beforeSend: function () {
            },
        });
    }
})
