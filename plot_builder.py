# Graph
import plotly
import plotly.express as px
from plotly.subplots import make_subplots
import plotly.graph_objects as go
import math
import colorsys
from itertools import cycle


def full_flowchart(
    traces_per_app: dict, label_from: str = "rhino", label_to: str = "rhino"
):

    # Generate initial graph
    trace = traces_per_app[label_from][label_to]
    options_y_send = ""
    options_x_receive = ""
    receive_apps = []
    for k, v in traces_per_app.items():
        options_y_send += f'<option value="{k}">{k}</option>'
    for k, v in traces_per_app.items():
        for kk, vv in v.items():
            if kk not in receive_apps:
                receive_apps.append(kk)
                options_x_receive += f'<option value="{kk}">{kk}</option>'

    layout = go.Layout(
        title="Graph Title",
        xaxis=dict(title="X-axis Title"),
        # yaxis=dict(title="Y-axis Title"),
    )

    fig = go.Figure(data=[trace], layout=layout)
    fig_html = fig.to_html(full_html=False)

    traces_html = ""
    count = 0
    for s, _ in traces_per_app.items():
        for r in receive_apps:
            try:
                html_trace = traces_per_app[s][r]
                if html_trace is not None:
                    fig_trace = go.Figure(data=[html_trace], layout=layout)
                    fig_trace = fig_trace.to_html(full_html=False)

                    fig_old_id = fig_trace.split('div id="')[-1].split('"')[0]
                    fig_new_id = f"plot_div_{s}_{r}"

                    traces_html += (
                        "<div>"
                        + fig_trace.replace(fig_old_id, fig_new_id).replace(
                            'class="plotly-graph-div" style="height:100%; width:100%;',
                            'class="plotly-graph-div" style="height:0px; width:100%;',
                        )
                        + "</div>"
                    )
                    count += 1
                    r'''
                    fig_trace = fig_trace.split("LICENSE.txt */")[-1].split(
                        "</script>"
                    )[0]
                    traces_html += f"""if (appSend === "{s}" && appReceive === "{r}") {{
                        var trace = {fig_trace};
                    }} 
                    """
                    '''
            except KeyError:
                pass

    # HTML template
    html_template = f"""
    <!DOCTYPE html>
    <html>
    <head>
        <script src="https://cdn.plot.ly/plotly-latest.min.js"></script>
    </head>
    <body>

    <div>
        <label for="send_app">Select Trace:</label>
        <select id="send_app" onchange="updateGraph()">
            {options_y_send}
        </select>
    </div>

    <div>
        <label for="receive_app">Select Option:</label>
        <select id="receive_app" onchange="updateGraph()">
            {options_x_receive}
        </select>
    </div>

    {traces_html}

    <script>
    function updateGraph() {{
        var sendApp = document.getElementById("send_app");
        var appSend = sendApp.options[sendApp.selectedIndex].value;

        var receiveApp = document.getElementById("receive_app");
        var appReceive = receiveApp.options[receiveApp.selectedIndex].value;

        var dateRE = /^plot_div_/;
        var divs=[],els=document.getElementsByTagName('*');
        for (var i=els.length;i--;) if (dateRE.test(els[i].id)) els[i].setAttribute("style","height:0px");

        var name = 'plot_div_' + appSend + '_' + appReceive
        console.log(name)

        document.getElementById(name).setAttribute("style","height:1000%; width:100%;");
    }}
    </script>

    </body>
    </html>
    """

    # Write HTML to file
    return html_template

    ############################ https://stackoverflow.com/questions/59406167/plotly-how-to-filter-a-pandas-dataframe-using-a-dropdown-menu
    fig = go.Figure()

    # set up ONE trace
    trace1 = traces_per_app[label_from][label_to]
    fig.add_trace(trace1)

    updatemenu = []
    buttons_from = []
    buttons_to = []

    # button with one option for each dataframe
    for from_app, val in traces_per_app.items():
        for to_app, graph in val.items():
            buttons_to.append(
                dict(
                    method="restyle",
                    label=to_app,
                    visible=True,
                    args=[graph, [0]],
                )
            )

    # some adjustments to the updatemenus
    updatemenu = []
    your_menu = dict()
    updatemenu.append(your_menu)

    updatemenu[0]["buttons"] = buttons_to
    updatemenu[0]["direction"] = "down"
    updatemenu[0]["showactive"] = True

    # add dropdown menus to the figure
    fig.update_layout(showlegend=False, updatemenus=updatemenu)
    return fig


def plot_flowchart(
    all_out_classes,
    all_sub_groups_sent,
    condition_1,
    all_sub_groups_received,
    condition_2,
    extra_receives: dict[str, dict[str, str]],
    title: str = "Title",
):

    cols = 2
    rows = 1

    specs_col = [
        {"type": "sunburst"},
    ]
    specs = [(specs_col + specs_col) for _ in range(rows)]
    fig = make_subplots(rows=rows, cols=cols, specs=specs)

    groups_send = []
    groups_receive = []
    groups_values = []
    groups_colors = []

    groups_extra_send = []
    groups_extra_mid = []
    groups_extra_receive = []
    groups_extra_values = []
    groups_extra_colors = []

    groups_first_labels = []

    extra_send = list(extra_receives.keys())
    extra_mid = [list(d.keys())[0] for d in list(extra_receives.values())]
    extra_receive = [list(v.values())[0] for v in extra_receives.values()]

    send_set = list(set(all_sub_groups_sent + extra_send))
    mid_set = list(set(extra_mid))
    receive_set = list(set(all_sub_groups_received + extra_receive))

    send_set_indices = []
    for gr_set in send_set:
        found = 0
        for i, gr in enumerate(all_sub_groups_sent):
            if gr == gr_set:
                send_set_indices.append(i)
                found = 1
                break
        if found == 0:
            send_set_indices.append(len(all_sub_groups_sent) + len(send_set_indices))

    receive_set_indices = []
    for gr_set in receive_set:
        found = 0
        for i, gr in enumerate(all_sub_groups_received):
            if gr == gr_set:
                receive_set_indices.append(i)
                found = 1
                break
        if found == 0:
            receive_set_indices.append(
                len(all_sub_groups_received) + len(receive_set_indices)
            )

    send_set = [x for _, x in sorted(zip(send_set_indices, send_set))]
    receive_set = [x for _, x in sorted(zip(receive_set_indices, receive_set))]
    send_set.reverse()
    receive_set.reverse()

    if len(send_set) == 0 or len(receive_set) == 0:
        return

    groups_first_labels = ["__" for _ in send_set]

    for j in range(len(groups_first_labels)):
        # insert empty node
        groups_send.append(j)
        groups_receive.append(len(groups_first_labels) + j)
        groups_values.append(1)
        groups_colors.append("white")

    for i, class_send in enumerate(all_sub_groups_sent):
        class_receive = all_sub_groups_received[i]

        # check if the app matches any of receiving group
        send_index = len(groups_first_labels) + send_set.index(class_send)  # 2nd column
        rec_index = (
            len(groups_first_labels) + len(send_set) + receive_set.index(class_receive)
        )  # 3rd column

        # first, check if already exist:
        exists = 0
        for k, _ in enumerate(groups_send):
            if send_index == groups_send[k] and rec_index == groups_receive[k]:
                # groups_values[k] += 1
                exists += 1
                break
        if exists == 0:
            groups_send.append(send_index)
            groups_receive.append(rec_index)
            groups_values.append(1)
            groups_colors.append(f"rgba(10,132,255,{0.7*condition_1[i]})")

    # extra columns
    for i, class_send in enumerate(extra_send):
        class_mid = extra_mid[i]
        class_receive = extra_receive[i]

        send_index = len(groups_first_labels) + send_set.index(class_send)  # 2nd column
        rec_index = (
            len(groups_first_labels) + len(send_set) + receive_set.index(class_receive)
        )  # 3rd column
        mid_index = (
            len(groups_first_labels)
            + len(send_set)
            + len(receive_set)
            + mid_set.index(class_mid)
        )

        groups_extra_send.append(send_index)
        groups_extra_mid.append(mid_index)
        groups_extra_receive.append(rec_index)
        groups_extra_values.append(1)
        groups_extra_colors.append("rgba(10,132,255,0.7)")

    ##### y-axis
    y_axis12 = [k / (len(send_set) - 0.9999999) + 0.001 for k, _ in enumerate(send_set)]
    y_axis3 = [
        k / (len(receive_set) - 0.9999999) + 0.001 for k, _ in enumerate(receive_set)
    ]
    if y_axis12[-1] > 1:
        y_axis12[-1] = 0.999
    y_axis12.reverse()
    if y_axis3[-1] > 1:
        y_axis3[-1] = 0.999
    y_axis3.reverse()
    y_axis_mid = [
        1 - ((k + 1) * 0.04 / (len(mid_set) + 0.001)) for k, _ in enumerate(mid_set)
    ]
    y_axis = y_axis12 + y_axis12 + y_axis3 + y_axis_mid

    #### x-axis
    x_axis = (
        [0.001 for _ in groups_first_labels]
        + [0.35 for _ in send_set]
        + [0.999 for _ in receive_set]
        + [0.6 for _ in mid_set]
    )

    mynode = dict(
        pad=15,
        thickness=2,
        line=dict(color="black", width=1.5),
        label=groups_first_labels + send_set + receive_set + mid_set,
        x=x_axis,
        y=y_axis,
        color="darkblue",
    )
    mylink = dict(
        source=groups_send
        + groups_extra_send
        + groups_extra_mid,  # indices correspond to labels, eg A1, A2, A1, B1, ...groups_send
        target=groups_receive + groups_extra_mid + groups_extra_receive,
        value=groups_values + groups_extra_values + groups_extra_values,
        color=groups_colors + groups_extra_colors + groups_extra_colors,
    )
    trace1 = go.Sankey(arrangement="snap", node=mynode, link=mylink)
    fig1 = go.Figure(data=[trace1])
    fig1.update_layout(title_text="Basic Sankey Diagram", font_size=20)

    fig.add_trace(fig1.data[0], row=1, col=1)

    fig.update_layout(title=title)

    width = 3600
    fig.update_layout(
        autosize=False,
        width=width,
        font_size=20,
        height=int(0.2 * width * max(len(send_set) / 15, 1)),
    )

    fig.update_yaxes(range=[0, 30], col=1)
    fig.update_xaxes(range=[0, 31 * 4], col=1)

    return trace1
    # plotly.offline.plot(fig, filename=f"flowchart_{title}.html")


# plot_flowchart(["1", "2", "1", "4", "5"], ["11", "22", "33", "11", "88"])

import plotly.graph_objects as go

# Sample data
x_values = [1, 2, 3, 4, 5]
y_values_1 = [10, 15, 13, 17, 18]
y_values_2 = [8, 11, 9, 12, 14]

# Generate initial graph
trace = go.Scatter(x=x_values, y=y_values_1, mode="lines+markers", name="Data")

layout = go.Layout(
    title="Graph Title",
    xaxis=dict(title="X-axis Title"),
    yaxis=dict(title="Y-axis Title"),
)

fig = go.Figure(data=[trace], layout=layout)
fig_html = fig.to_html(full_html=False)

# HTML template
html_template = f"""
<!DOCTYPE html>
<html>
<head>
    <script src="https://cdn.plot.ly/plotly-latest.min.js"></script>
</head>
<body>

<div>
    <label for="trace_select">Select Trace:</label>
    <select id="trace_select" onchange="updateGraph()">
        <option value="trace1">Trace 1</option>
        <option value="trace2">Trace 2</option>
    </select>
</div>

<div>
    <label for="option_select">Select Option:</label>
    <select id="option_select" onchange="updateGraph()">
        <option value="option1">Option 1</option>
        <option value="option2">Option 2</option>
    </select>
</div>

<div id="plot_div">
    {fig_html}
</div>

<script>
function updateGraph() {{
    var traceSelect = document.getElementById("trace_select");
    var selectedTrace = traceSelect.options[traceSelect.selectedIndex].value;

    var optionSelect = document.getElementById("option_select");
    var selectedOption = optionSelect.options[optionSelect.selectedIndex].value;

    var xValues;
    var yValues;
    if (selectedTrace === "trace1") {{
        yValues = {y_values_1};
    }} else {{
        yValues = {y_values_2};
    }}

    if (selectedOption === "option1") {{
        xValues = {x_values};
    }} else {{
        // Example of different x values for option 2
        xValues = [1, 3, 5, 7, 9];
    }}

    var trace = {{
        x: xValues,
        y: yValues,
        mode: 'lines+markers',
        name: 'Data'
    }};

    var layout = {{
        title: 'Graph Title',
        xaxis: {{ title: 'X-axis Title' }},
        yaxis: {{ title: 'Y-axis Title' }}
    }};

    Plotly.newPlot('plot_div', [trace], layout);
}}
</script>

</body>
</html>
"""

# Write HTML to file
with open("plotly_graph.html", "w", encoding="utf-8") as html_file:
    html_file.write(html_template)
