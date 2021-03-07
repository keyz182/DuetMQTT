# Duet to MQTT

Sends the Duet object model to MQTT.

Extract the release (or copy build artifacts) to `/opt/DuetMQTT`.
Copy `duetmqtt.service` to `/etc/systemd/service/`. 
Run:
```
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