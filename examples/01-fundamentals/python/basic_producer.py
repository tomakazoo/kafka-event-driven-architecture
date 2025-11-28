from confluent_kafka import Producer
import json
import sys


def delivery_report(err, msg):
    if err is not None:
        print(f'Message delivery failed: {err}')
    else:
        print(f'Message delivered to {msg.topic()} [{msg.partition()}]')


def main():
    conf = {
        'bootstrap.servers': 'localhost:9092',
        'client.id': 'python-producer'
    }
    producer = Producer(conf)

    # Produce messages
    topic = 'my-topic'
    for i in range(5):
        message = {
            'id': i,
            'value': f'Message {i}'
        }
        producer.produce(
            topic,
            key=f'key-{i}',
            value=json.dumps(message).encode('utf-8'),
            callback=delivery_report
        )
    producer.flush()

    print('Producer completed')


if __name__ == '__main__':
    main()


