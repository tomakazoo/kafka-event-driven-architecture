from confluent_kafka import Consumer
import signal
import sys


def main():
    conf = {
        'bootstrap.servers': 'localhost:9092',
        'group.id': 'python-consumer-group',
        'auto.offset.reset': 'earliest',
        'enable.auto.commit': True
    }
    consumer = Consumer(conf)
    consumer.subscribe(['my-topic'])

    running = True

    def signal_handler(sig, frame):
        nonlocal running
        running = False

    signal.signal(signal.SIGINT, signal_handler)

    try:
        while running:
            msg = consumer.poll(timeout=1.0)
            if msg is None:
                continue
            if msg.error():
                print(f'Error: {msg.error()}')
            else:
                print(f'Received: {msg.value().decode("utf-8")}')
    finally:
        consumer.close()
        print('Consumer stopped')


if __name__ == '__main__':
    main()













