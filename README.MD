# Duet to MQTT

Sends the Duet object model to MQTT.

Extract the release (or copy build artifacts) to `/opt/DuetMQTT`.
Copy `duetmqtt.service` to `/etc/systemd/service/`. 
Run:
```
sudo chmod +x /opt/DuetMQTT/DuetMQTT
sudo systemctl daemon-reload
sudo systemctl enable duetmqtt
sudo systemctl start duetmqtt
```

```
./DuetMQTT --help
DuetMQTT 1.0.0
Copyright (C) 2021 DuetMQTT
  -q, --quiet       (Default: false) Suppress output
  -s, --socket      (Default: /var/run/dsf/dcs.sock) UNIX socket to connect to.
  -t, --topic       (Default: duet/printer) The topic to send to.
  -h, --host        (Default: localhost) The MQTT Host.
  -p, --port        (Default: 1883) The MQTT Port.
  --username        The MQTT User.
  --password        The MQTT Password.
  --tls             (Default: false) Use TLS.
  -i, --interval    (Default: 1000) The Interval between calls to duet.
  --drop-thumbs     (Default: false) Drop Thumbnails from the payload to reduce size.
  --help            Display this help screen.
  --version         Display version information.
```

## Home Assistant example

Start `DuetMQTT` with this command, replacing `myprinter` and `mqtthost`:
```/opt/DuetMQTT/DuetMQTT -s /var/run/dsf/dcs.sock -t "duet/myprinter" -h mqtthost --drop-thumbs --quiet```

Add the following to your Home Assistant `configuration.yaml`:

```yaml

sensor:
  - platform: mqtt
    device_class: "temperature"
    unique_id: "myprinter.h0.current"
    name: "MyPrinter Heater0 Current"
    state_topic: "duet/myprinter"
    unit_of_measurement: "°C"
    value_template: "{{ value_json.heat.heaters[0].current }}"
  - platform: mqtt
    device_class: "temperature"
    unique_id: "myprinter.h0.active"
    name: "MyPrinter Heater0 Active"
    state_topic: "duet/myprinter"
    unit_of_measurement: "°C"
    value_template: "{{ value_json.heat.heaters[0].active }}"
  - platform: mqtt
    device_class: "temperature"
    unique_id: "myprinter.h1.current"
    name: "MyPrinter Heater1 Current"
    state_topic: "duet/myprinter"
    unit_of_measurement: "°C"
    value_template: "{{ value_json.heat.heaters[1].current }}"
  - platform: mqtt
    device_class: "temperature"
    unique_id: "myprinter.h1.active"
    name: "MyPrinter Heater1 Active"
    state_topic: "duet/myprinter"
    unit_of_measurement: "°C"
    value_template: "{{ value_json.heat.heaters[1].active }}"
  - platform: mqtt
    device_class: "voltage"
    unique_id: "myprinter.v_in.current"
    name: "MyPrinter vIn Current"
    state_topic: "duet/myprinter"
    unit_of_measurement: "v"
    value_template: "{{ value_json.boards[0].vIn.current }}"
  - platform: mqtt
    device_class: "voltage"
    unique_id: "myprinter.v_in.min"
    name: "MyPrinter vIn Min"
    state_topic: "duet/myprinter"
    unit_of_measurement: "v"
    value_template: "{{ value_json.boards[0].vIn.min }}"
  - platform: mqtt
    device_class: "voltage"
    unique_id: "myprinter.v_in.max"
    name: "MyPrinter vIn Max"
    state_topic: "duet/myprinter"
    unit_of_measurement: "v"
    value_template: "{{ value_json.boards[0].vIn.max }}"
  - platform: mqtt
    unique_id: "myprinter.job_duration"
    name: "MyPrinter Job Duration"
    state_topic: "duet/myprinter"
    unit_of_measurement: "s"
    value_template: "{{ value_json.job.duration }}"
  - platform: mqtt
    unique_id: "myprinter.job_timeleft_filament"
    name: "MyPrinter Job Time Left Filament"
    state_topic: "duet/myprinter"
    unit_of_measurement: "s"
    value_template: "{{ value_json.job.timesleft.filament }}"
  - platform: mqtt
    unique_id: "myprinter.job_timeleft_file"
    name: "MyPrinter Job Time Left File"
    state_topic: "duet/myprinter"
    unit_of_measurement: "s"
    value_template: "{{ value_json.job.timesleft.file }}"
  - platform: mqtt
    unique_id: "myprinter.job_timeleft_layer"
    name: "MyPrinter Job Time Left Layer"
    state_topic: "duet/myprinter"
    unit_of_measurement: "s"
    value_template: "{{ value_json.job.timesleft.layer }}"
  - platform: mqtt
    unique_id: "myprinter.job_percent_filament"
    name: "MyPrinter Job (Filament)"
    state_topic: "duet/myprinter"
    unit_of_measurement: "%"
    value_template: "{{ value_json.job.timesleft.filament / (value_json.job.timesleft.filament + value_json.job.duration) }}"
  - platform: mqtt
    unique_id: "myprinter.job_percent_file"
    name: "MyPrinter Job (File)"
    state_topic: "duet/myprinter"
    unit_of_measurement: "%"
    value_template: "{{ value_json.job.timesleft.file / (value_json.job.timesleft.file + value_json.job.duration) }}"
  - platform: mqtt
    unique_id: "myprinter.job_percent_layer"
    name: "MyPrinter Job (Layer)"
    state_topic: "duet/myprinter"
    unit_of_measurement: "%"
    value_template: "{{ value_json.job.timesleft.layer / (value_json.job.timesleft.layer + value_json.job.duration) }}"
```

These entities will be availabe then in HomeAssistant to use as you wish.

![example of entities in home assistant](https://raw.githubusercontent.com/keyz182/DuetMQTT/main/hass.png "example of entities in home assistant")
